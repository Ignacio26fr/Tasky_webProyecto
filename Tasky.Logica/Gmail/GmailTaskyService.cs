using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Tasky.Logica.Redis;

namespace Tasky.Logica.Gmail;

public interface IGmailTaskyService
{
    Task<List<EmailInfo>> GetInbox(string accessToken, int limit);
    Task<List<EmailInfo>> GetEmailsFromHistoryId(string accessToken, ulong historyId);
    Task<ulong> SyncHistoryInit(string accessToken);
}

public class GmailTaskyService : IGmailTaskyService
{

    private readonly IRedisSessionService _redisSessionService;
    public GmailTaskyService(IRedisSessionService redisSessionService)
    {
        _redisSessionService = redisSessionService;
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

            var historyRequest = service.Users.History.List("osnaghi.developer@gmail.com");

            var LastHistoryId = await this.GetLastHistoryId(accessToken);

            historyRequest.StartHistoryId = LastHistoryId;
            var historyResponse = await historyRequest.ExecuteAsync();

            if (historyResponse.HistoryId > LastHistoryId)
            {
                //alaceno el historyId en la sesion Redis
                await _redisSessionService.SetValueAsync("goo_history_id", historyResponse.HistoryId.Value.ToString());
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
            var historyRequest = service.Users.History.List("me");
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
                            var email = await service.Users.Messages.Get("me", messageAdded.Message.Id).ExecuteAsync();

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
                await _redisSessionService.SetValueAsync("goo_history_id", email.HistoryId!.Value.ToString());
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
}
