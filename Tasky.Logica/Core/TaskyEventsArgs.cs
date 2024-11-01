using Tasky.Datos.EF;

namespace Tasky.Logica.Core;

public class TaskEventsArgs : EventArgs
{
    public TaskEventsArgs(TaskyObject task)
    {
        TaskObject = task ;
    }
    public TaskyObject TaskObject { get; set; }
}