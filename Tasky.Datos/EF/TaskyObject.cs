
namespace Tasky.Datos.EF;


public class TaskyObject
{
    public int IdObject { get; set; }
    public string Subjectt { get; set; }
    public string Sender { get; set; }
    public string Body { get; set; }
    public DateTime Date { get; set; }
    public int IdStatus { get; set; }
    public bool Spam { get; set; }
    public int IdPriority { get; set; }


    public TaskyStatus Status { get; set; }
    public TaskyPriority Priority { get; set; }
}