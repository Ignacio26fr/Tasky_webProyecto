namespace Tasky.Entidad.Actions;

public enum TaskStatus
{
    Pending,
    Completed
}

public enum TaskPriority
{
    Baja,
    Media,
    Urgente
}

public class Task
{
    public string Id { get; set; }
    public string Subject { get; set; }
    public string Sender { get; set; } 
    public string Body { get; set; } 
    public DateTime Date { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
}
