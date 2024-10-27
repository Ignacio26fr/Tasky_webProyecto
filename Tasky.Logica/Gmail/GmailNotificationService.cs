using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Tasky.Entidad.GmailAccount.PubSub;
using Tasky.Logica.Core;
using Tasky.Logica.Redis;

namespace Tasky.Logica.Gmail;

public interface IGmailNotificationService
{
    Task ProcessNotification(PubSubNotification notification);
    Task<string> SubscribeToNotifications(string accessToken);
    Task UnsubscribeFromNotifications(string accessToken, string subscriptionId);
    void AddNotification(PubSubNotification notification);
}


public class GmailNotificationService : IGmailNotificationService
{
    private readonly IGmailTaskyService _gmailTaskyService;
    private readonly IRedisSessionService _redisSessionService;
    private readonly ConcurrentQueue<PubSubNotification> _notificationQueue;
    private readonly ITaskManager _taskManager;
    private GmailService _gmailService;
    private bool _isProcessing;
    private string accessToken;
    private string _accessToken;

    public GmailNotificationService(IGmailTaskyService gmailService, IRedisSessionService redisSessionService, ITaskManager taskManager)
    {
        _gmailTaskyService = gmailService;
        _redisSessionService = redisSessionService;
        _notificationQueue = new ConcurrentQueue<PubSubNotification>();
        _taskManager = taskManager;
        _isProcessing = false;
    }

    private async Task<string> GetAccessToken()
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            _accessToken = await _redisSessionService.GetValueAsync("goo_access_token");
        }
        return _accessToken;
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
        var accessToken = await GetAccessToken();

        var result = new List<EmailInfo>();
        try
        {

            var lastHistoryId = await _redisSessionService.GetValueAsync("goo_history_id");
            result = await Process(notification, accessToken, ulong.Parse(lastHistoryId));

            //TODO: mandar la lista de correos a ML para procesar
            //TODO: mandar la lista de correos a la base de datos


            Console.WriteLine($"Total mails conseguidos:{result.Count}");




        }
        catch (Exception ex)
        {
            await UnsubscribeFromNotifications(accessToken, notification.Subscription);

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
                var newEmails = await _gmailTaskyService.GetEmailsFromHistoryId(accessToken, lastHistoryId);

                //si obtubimos nuevos correos, actualizamos el historyId
                if (newEmails.Count > 0)
                {
                   
                    //alaceno el historyId en la sesion Redis
                    await _redisSessionService.SetValueAsync("goo_history_id", actualHistoryId.ToString());
                    
                    //mostramos el primer correo de newEmail
                    foreach (var email in newEmails)
                    {
                        Console.WriteLine("CORREO ENTRANTE");
                        Console.WriteLine("================");
                        Console.WriteLine($"Correo-id: {email.Id}");
                        Console.WriteLine($"Correo-Hid: {email.HistoryId}");
                        Console.WriteLine($"Correo-asunto: {email.Subject}");
                        Console.WriteLine($"Correo-fecha: {email.Date}");
                        Console.WriteLine($"Correo-remite: {email.Sender}");
                        Console.WriteLine($"Correo-body: {email.Body}");

                        _taskManager.GenerateTaskFromEmail(email);
                    }




                }

                return newEmails;

            }
        }

        return new List<EmailInfo>();
    }

    public async Task<string> SubscribeToNotifications(string accessToken)
    {

        var hId = await _gmailTaskyService.SyncHistoryInit(accessToken);
        Console.WriteLine($"HistoryId de sincronizacion: {hId}");

        _gmailService = new GmailService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken),
            ApplicationName = "Tasky",
        });

        var request = new WatchRequest
        {
            LabelIds = new List<string> { "INBOX" },
            TopicName = "projects/tasky-439413/topics/notifications",
        };



        var response = await _gmailService.Users.Watch(request, "me").ExecuteAsync();


        var historyId = response.HistoryId;

        Console.WriteLine($"HistoryId de Gmail: {historyId}");




        return "projects/tasky-439413/subscriptions/notification-sub"; // Este es el subscriptionId
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

            var stopRequest = _gmailService.Users.Stop("me");
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

}
