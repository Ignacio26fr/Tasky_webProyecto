namespace Tasky.Logica.Gmail;


public class EmailInfo
{

    public string Id { get; set; }
    public ulong HistoryId { get; set; }
    public string Subject { get; set; }
    public string Sender { get; set; } // Remitente
    public string Body { get; set; } // Cuerpo del mensaje
    public DateTime Date { get; set; }

    public string UserId { get; set; }

}
