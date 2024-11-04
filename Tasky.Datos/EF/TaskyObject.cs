
namespace Tasky.Datos.EF;

public enum TaskyPriority
{
    Baja = 0,
    Media = 1,
    Alta = 2
}

public class TaskyObject
{
    public string IdObject { get; set; }
    public string Subjectt { get; set; }
    public string Sender { get; set; }
    public string Body { get; set; }
    public DateTime Date { get; set; }
    public bool Spam { get; set; }
    public string UserId { get; set; }
    public bool Status { get; set; }
    public TaskyPriority Priority { get; set; }

    public bool Delete { get; set; }

    public DateTime ExpectData { get; set; }
}