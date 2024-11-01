using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Tasky.Datos.EF;
using Tasky.Entidad.GmailAccount.PubSub;
using Tasky.Logica.Core;

namespace Tasky.Logica.Gmail;

public interface IGmailNotificationService
{
    
    GoogleSession CurrenSession {  set; }
    Task ProcessNotification(PubSubNotification notification);
    Task<string> SubscribeToNotificationsAsync(GoogleSession session);
    Task UnsubscribeFromNotifications(string accessToken, string subscriptionId);
    void AddNotification(PubSubNotification notification);
}


public class GmailNotificationService : IGmailNotificationService
{
    
    private  readonly ConcurrentQueue<PubSubNotification> _notificationQueue;
    private readonly ICoreBackgroundService _coreBackgroundService;
    private GmailService _gmailService;
    private bool _isProcessing;
   
    public GoogleSession CurrenSession { private get; set; }

    public GmailNotificationService(ICoreBackgroundService coreBackgroundService)
    {

        _coreBackgroundService = coreBackgroundService;
        _isProcessing = false;
        _notificationQueue = new ConcurrentQueue<PubSubNotification>();
    }

    public void AddNotification(PubSubNotification notification)
    {
        _notificationQueue.Enqueue(notification);
        ProcessQueue();
    }
    private void ProcessQueue()
    {



        // Evitar que múltiples hilos procesen la cola simultáneamente
        if (_isProcessing)
        {
            return;
        }

        _isProcessing = true;

        Task.Run(async () =>
        {
            while (_notificationQueue.TryDequeue(out var notification))
            {
                try
                {
                    await ProcessNotification(notification);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando notificación: {ex.Message}");
                    // Manejo de errores o lógica de reintentos.
                }
            }

            _isProcessing = false;
        });
    }
    public async Task ProcessNotification(PubSubNotification notification)
    {

        var result = new List<EmailInfo>();
        try
        {

            var lastHistoryId = CurrenSession.User.GoogleHistoryId;
            result = await Process(notification, CurrenSession.AccessToken,(ulong?) lastHistoryId ?? 0);

            //TODO: mandar la lista de correos a ML para procesar
            //TODO: mandar la lista de correos a la base de datos


            Console.WriteLine($"Total mails conseguidos:{result.Count}");




        }
        catch (Exception ex)
        {
            await UnsubscribeFromNotifications(CurrenSession.AccessToken, notification.Subscription!);

            Console.WriteLine($"error al procesar la notificacion: {ex}");
        }
    }
    private async Task<List<EmailInfo>> Process(PubSubNotification notification, string accessToken, ulong lastHistoryId)
    {

        if (notification?.Message?.Data != null)
        {
            // Decodificar el campo "data" desde Base64
            var decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(notification.Message.Data));

            // Deserializar el JSON para obtener la información de la notificación de Gmail
            var gmailNotification = JsonSerializer.Deserialize<GmailNotificationData>(decodedData);

            if (gmailNotification != null)
            {
                var actualHistoryId = gmailNotification!.HistoryId;

                // Utilizo el HistoryId para buscar los nuevos correos por medio del servicio de gmail
                var newEmails = await GetEmailsFromHistoryId(accessToken, lastHistoryId);

                //si obtubimos nuevos correos, actualizamos el historyId
                if (newEmails.Count > 0)
                {
                   
                    //alaceno el historyId en los datos del usuario
                    await SaveHistoryId(actualHistoryId);


                    //mostramos el primer correo de newEmail
                    foreach (var email in newEmails)
                    {
                       _coreBackgroundService.AddEmail(email);
                    }




                }

                return newEmails;

            }
        }

        return new List<EmailInfo>();
    }

    public async Task<string> SubscribeToNotificationsAsync(GoogleSession session)
    {
        CurrenSession = session;

        var hId = await SyncHistoryInit(CurrenSession.AccessToken);
        Console.WriteLine($"HistoryId de sincronizacion: {hId}");

        _gmailService = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(CurrenSession.AccessToken),
            ApplicationName = "Tasky",
        });

        var request = new WatchRequest
        {
            LabelIds = new List<string> { "INBOX" },
            TopicName = "projects/tasky-439320/topics/notifications",
        };



        var response = await _gmailService.Users.Watch(request, "me").ExecuteAsync();


        var historyId = response.HistoryId;

        Console.WriteLine($"HistoryId de Gmail: {historyId}");




        return "projects/tasky-439320/subscriptions/notification-sub"; // Este es el subscriptionId
    }

    // Lógica para cancelar la suscripción
    public async Task UnsubscribeFromNotifications(string accessToken, string subscriptionId)
    {
        try
        {
            _gmailService = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
                ApplicationName = "Tasky",
            });

            var stopRequest = _gmailService.Users.Stop(CurrenSession.User.Email);
            await stopRequest.ExecuteAsync();
            Console.WriteLine("Unsubscribed from notifications");
        }
        catch (Google.GoogleApiException ex)
        {
            Console.WriteLine($"Google API Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }


    }

    public async Task<List<EmailInfo>> GetInbox(string accessToken, int limit)
    {
        var service = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
            ApplicationName = "Tasky",
        });

        var request = service.Users.Messages.List("me");
        request.MaxResults = limit; // Número máximo de correos a recuperar

        ListMessagesResponse response = await request.ExecuteAsync();

        var emails = new List<EmailInfo>();

        if (response.Messages != null)
        {
            foreach (var message in response.Messages)
            {
                var email = await service.Users.Messages.Get("me", message.Id).ExecuteAsync();

                if (email != null)
                {
                    var emailInfo = EmailInfoAdapter(email);
                    emails.Add(emailInfo);
                }
            }
        }
        //retornamos la lista de correos
        return emails;
    }

    public async Task<ulong> SyncHistoryInit(string accessToken)
    {
        try
        {
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
                ApplicationName = "Tasky",
            });

            var historyRequest = service.Users.History.List("me");

            var LastHistoryId = await this.GetLastHistoryId(accessToken);

            historyRequest.StartHistoryId = LastHistoryId;
            var historyResponse = await historyRequest.ExecuteAsync();

            if (historyResponse.HistoryId > LastHistoryId)
            {
                //alaceno el historyId en los datos del CurrentUser
                await SaveHistoryId(historyResponse.HistoryId.Value);
            }

            return historyResponse.HistoryId.Value;
        }
        catch (GoogleApiException ex)
        {
            Console.WriteLine($"Google API Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public async Task<List<EmailInfo>> GetEmailsFromHistoryId(string accessToken, ulong historyId)
    {
        try
        {
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
                ApplicationName = "Tasky",
            });

            // Buscar los correos nuevos a partir del HistoryId
            var historyRequest = service.Users.History.List(CurrenSession.User.Email);
            historyRequest.StartHistoryId = historyId;
            var historyResponse = await historyRequest.ExecuteAsync();
            var emails = new List<EmailInfo>();

            if (historyResponse.History != null)
            {
                foreach (var history in historyResponse.History)
                {
                    if (history.MessagesAdded != null)
                    {
                        foreach (var messageAdded in history.MessagesAdded)
                        {
                            
                            var email = await service.Users.Messages.Get(CurrenSession.User.Email, messageAdded.Message.Id).ExecuteAsync();

                            if (email != null)
                            {
                                var emailInfo = EmailInfoAdapter(email);
                                emails.Add(emailInfo);
                            }
                        }
                    }
                }
            }




            return emails;
        }
        catch (GoogleApiException ex)
        {
            Console.WriteLine($"Google API Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

    }

    private EmailInfo EmailInfoAdapter(Message email)
    {
        return new EmailInfo()
        {
            UserId = CurrenSession.User.Id,
            HistoryId = email.HistoryId!.Value,
            Id = email.Id,
            Subject = email.Payload.Headers.FirstOrDefault(h => h.Name == "Subject")!.Value,
            Sender = email.Payload.Headers.FirstOrDefault(h => h.Name == "From")!.Value,
            Date = email.InternalDate.HasValue ?
                                 DateTimeOffset.FromUnixTimeMilliseconds(email.InternalDate.Value).DateTime :
                                 DateTime.Now,
            Body = GetEmailBody(email)
        };
    }

    private async Task<ulong?> GetLastHistoryId(string accessToken)
    {
        try
        {
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
                ApplicationName = "Tasky",
            });

            var request = service.Users.Messages.List("me");
            request.MaxResults = 1;

            ListMessagesResponse response = await request.ExecuteAsync();

            if (response.Messages != null)
            {

                // Obtenemos el historyId del primer correo
                var email = await service.Users.Messages.Get("me", response.Messages[0].Id).ExecuteAsync();
                //alaceno el historyId en la sesion Redis
                await SaveHistoryId(email.HistoryId!.Value);
                Console.WriteLine($"Ultimo HistoryId [GetLastHistoryId]: {email.HistoryId!.Value}");
                return email.HistoryId.Value;

            }

            return null;
        }
        catch (GoogleApiException ex)
        {
            Console.WriteLine($"Google API Error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private string GetEmailBody(Message email)
    {
        if (email.Payload.Parts == null || !email.Payload.Parts.Any())
        {
            // Si no hay partes, obtenemos el cuerpo directamente del Payload.Body
            var decodedBody = Base64UrlDecode(email.Payload.Body.Data);
            return decodedBody;
        }

        // Si el mensaje tiene varias partes
        foreach (var part in email.Payload.Parts)
        {
            if (!string.IsNullOrEmpty(part.MimeType) && part.MimeType == "text/plain")
            {
                return Base64UrlDecode(part.Body.Data);
            }
            else if (!string.IsNullOrEmpty(part.MimeType) && part.MimeType == "text/html")
            {
                return Base64UrlDecode(part.Body.Data);
            }
        }

        return string.Empty;
    }

    // Decodificación de Base64Url
    private string Base64UrlDecode(string input)
    {
        var base64EncodedBytes = Convert.FromBase64String(input.Replace("-", "+").Replace("_", "/"));
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }


    private async Task SaveHistoryId(ulong historyId)
    {
        CurrenSession.User.GoogleHistoryId = (long)historyId;
       // HistoryChange?.Invoke(this, new HistoryArgs(CurrenSession.User, historyId));
       _coreBackgroundService.SaveHistoryId(CurrenSession.User, historyId);

    }

}
