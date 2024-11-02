using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tasky.Datos.EF;
using Twilio.Rest.Trunking.V1;
namespace Tasky.Logica.Core;

public interface ITaskManager
{
  Task<List<TaskyObject>> GetAllTasksAsync();
    Task<List<TaskyObject>> GetTasksForPriority(TaskyPriority? idPriority);
    Task<List<TaskyObject>> GetTasksForToday(TaskyPriority? taskyPriority);
    Task<List<TaskyObject>> GetTasksSpam(TaskyPriority? taskyPriority);
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

    public async Task<List<TaskyObject>> GetAllTasksAsync()
    {
         var tasks = await _taskyContext.TaskyObjects.ToListAsync();
        return tasks;
    }

    public async Task<List<TaskyObject>> GetTasksForPriority(TaskyPriority? idPriority)
    {



        var tasks = idPriority != null
            ? await _taskyContext.TaskyObjects.Where(t => t.Priority == idPriority && !t.Spam)
                                                     .OrderByDescending(t => t.Date)
                                                    .ToListAsync()
            : await _taskyContext.TaskyObjects.Where(t => !t.Spam).OrderByDescending(t => t.Date).ToListAsync();


        return tasks;
    }

    public async Task<List<TaskyObject>> GetTasksForToday(TaskyPriority? taskyPriority)
    {
        

       var tasks = taskyPriority != null
            ? await _taskyContext.TaskyObjects.Where(t => t.Date.Date == DateTime.Now.Date && t.Priority == taskyPriority 
                                                    && !t.Spam)
                                               .OrderByDescending(t => t.Date)
                                                .ToListAsync()
            : await _taskyContext.TaskyObjects.Where(t => t.Date.Date == DateTime.Now.Date && !t.Spam)
                                               .OrderByDescending(t => t.Date)
                                                .ToListAsync();


        return tasks;
    }

    public async Task<List<TaskyObject>> GetTasksSpam(TaskyPriority? taskyPriority)
    {

        

       var tasks = taskyPriority != null
            ? await _taskyContext.TaskyObjects.Where(t => t.Spam == true && t.Priority == taskyPriority)
                                               .OrderByDescending(t => t.Date)
                                                .ToListAsync()
            : await _taskyContext.TaskyObjects.Where(t => t.Spam == true)
                                               .OrderByDescending(t => t.Date)
                                                .ToListAsync();


        return tasks;
    }
}