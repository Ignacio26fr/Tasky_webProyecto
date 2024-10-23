using System.Text.Json.Serialization;


namespace Tasky.Entidad.GmailAccount.PubSub;

public class PubSubNotification
{
    public PubSubMessage? Message { get; set; }
    public string? Subscription { get; set; }
}

public class PubSubMessage
{
    public string? Data { get; set; } // Este es el campo que contiene el contenido Base64
    public string? MessageId { get; set; }
    public DateTime PublishTime { get; set; }
}

public class GmailNotificationData
{
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; } // El email asociado
    [JsonPropertyName("historyId")]
    public ulong HistoryId { get; set; } // El ID del historial para buscar correos nuevos
}
