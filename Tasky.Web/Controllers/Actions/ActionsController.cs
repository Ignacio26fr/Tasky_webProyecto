using Microsoft.AspNetCore.Mvc;
using Tasky.Datos.EF;
using Tasky.Logica.Core;
using Tasky.Logica.Gmail;



namespace Tasky.Web.Controllers.Actions;

public class ActionsController : Controller
{

    private readonly ITaskManager _taskManager;
  

    public ActionsController( ITaskManager taskManager)
    {
        _taskManager = taskManager;
     
    }

    public async Task<IActionResult> Index(TaskyPriority? idPrioridad, string filtroInput, string filtroSeccion)
    {

        ViewBag.Prioridades = Enum.GetValues(typeof(TaskyPriority)).Cast<TaskyPriority>().ToList();
        ViewBag.PrioridadSeleccionada = idPrioridad;
        ViewBag.FiltroSeccion = filtroSeccion;

        List<TaskyObject> tasks = await _taskManager.GetTasksForPriority(idPrioridad);

        if(!string.IsNullOrEmpty(filtroInput))
        {
            tasks = tasks.Where(x => x.Subjectt.Contains(filtroInput) || x.Sender.Contains(filtroInput)).ToList();
        }

        return View(tasks);
    }

    public async Task<IActionResult> Hoy(TaskyPriority? idPrioridad, string filtroInput)
    {
        ViewBag.Prioridades = Enum.GetValues(typeof(TaskyPriority)).Cast<TaskyPriority>().ToList();
        ViewBag.PrioridadSeleccionada = idPrioridad;
        ViewBag.FiltroSeccion = "hoy";

        List<TaskyObject> tasks = await _taskManager.GetTasksForToday(idPrioridad);

        if (!string.IsNullOrEmpty(filtroInput))
        {
            tasks = tasks.Where(x => x.Subjectt.Contains(filtroInput) || x.Sender.Contains(filtroInput)).ToList();
        }

        ViewBag.FiltroInput = filtroInput;
        return View("Index", tasks); 
    }

    public async Task<IActionResult> Spam(TaskyPriority? idPrioridad, string filtroInput)
    {
        ViewBag.Prioridades = Enum.GetValues(typeof(TaskyPriority)).Cast<TaskyPriority>().ToList();
        ViewBag.PrioridadSeleccionada = idPrioridad;
        ViewBag.FiltroSeccion = "spam";

        List<TaskyObject> tasks = await _taskManager.GetTasksSpam(idPrioridad);

        if (!string.IsNullOrEmpty(filtroInput))
        {
            tasks = tasks.Where(x => x.Subjectt.Contains(filtroInput) || x.Sender.Contains(filtroInput)).ToList();
        }

        ViewBag.FiltroInput = filtroInput;
        return View("Index", tasks);
    }


}



 



