using Microsoft.Extensions.DependencyInjection;
using Tasky.Datos.EF;
namespace Tasky.Logica.Core;

public interface ITaskManager
{
  

}

public class TaskManager : ITaskManager, IObserver<TaskEventsArgs>
{
  
   
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TaskyContext _taskyContext;
    private readonly IDisposable _taskSubscription;


    public TaskManager(IServiceScopeFactory scopeFactory, TaskyContext taskyContext, IEventService<TaskEventsArgs> taskEventService)
    {
        _taskyContext = taskyContext;
        _scopeFactory = scopeFactory;
        _taskSubscription = taskEventService.Subscribe(this);


    }


    public void OnNext(TaskEventsArgs e)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var taskyContext = scope.ServiceProvider.GetRequiredService<TaskyContext>();
            taskyContext.TaskyObjects.Add(e.TaskObject);
            taskyContext.SaveChanges();
        }
        Console.WriteLine("Tarea guardada en base de datos");
       
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }

    public void Dispose()
    {
        _taskSubscription.Dispose();
       
    }

    
}