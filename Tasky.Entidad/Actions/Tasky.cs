namespace Tasky.Entidad.Actions;

public enum TaskyStatus
{
    Pendiente,
    Completada
}

public enum TaskyPriority
{
    Baja,
    Media,
    Urgente
}

public class TaskyObject
{
    public string Id { get; set; }
    public string Subject { get; set; }
    public string Sender { get; set; } 
    public string Body { get; set; } 
    public DateTime Date { get; set; }
    public TaskyStatus Status { get; set; }
    public bool Spam { get; set; }
    public TaskyPriority Priority { get; set; }
}
