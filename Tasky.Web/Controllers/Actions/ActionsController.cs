using Microsoft.AspNetCore.Mvc;
using Tasky.Datos.EF;
using Tasky.Logica.Calendar;
using Tasky.Logica.Core;
using Tasky.Logica.Gmail;
using Tasky.Web.Models;



namespace Tasky.Web.Controllers.Actions;

public class ActionsController : Controller
{

    private readonly ITaskManager _taskManager;

    private readonly IConfiguration _configuration;

    private readonly IGoogleCalendarService _googleCalendarService;


 
    public ActionsController( ITaskManager taskManager, IGoogleCalendarService googleCalendarService , IConfiguration configuration)
    {
        _taskManager = taskManager;
        _googleCalendarService = googleCalendarService;
         _configuration = configuration;

    }

    public async Task<IActionResult> Index(TaskyPriority? priority, string filtroInput)
    {
        //necesario para menu
        ViewBag.MenuItems = _configuration.GetSection("MenuItems")
                                            .Get<List<MenuViewModel>>()
                                            .FindAll(m => m.Controller == "Actions");
        ViewBag.Controller = "Actions";
        ViewBag.Action = "Index";
        ViewBag.CurrentSection = _configuration.GetSection("MenuItems")
                                                .Get<List<MenuViewModel>>()
                                                .Find(m => m.Controller == "Actions" && m.Action == "Index")?
                                                .Name;


        ViewBag.Prioridades = Enum.GetValues(typeof(TaskyPriority)).Cast<TaskyPriority>().ToList();
        ViewBag.PrioridadSeleccionada = priority;
        ViewBag.DefaultComboText = "Todas";
        ViewBag.FiltroInput = filtroInput;
        ViewBag.EnableDynamicSubmit = true;

        List<TaskyObject> tasks = await _taskManager.GetTasksForPriority(priority);

        if(!string.IsNullOrEmpty(filtroInput))
        {
            tasks = tasks.Where(x => x.Subjectt.ToLower().Contains(filtroInput.ToLower()) || x.Sender.ToLower().Contains(filtroInput.ToLower())).ToList();
        }

        if (priority != null)
        {
            tasks = tasks.FindAll(x => x.Priority == priority);
        }

        return View(tasks);

    }

    public async Task<IActionResult> Hoy(TaskyPriority? priority, string filtroInput)
    {
        ViewBag.Prioridades = Enum.GetValues(typeof(TaskyPriority)).Cast<TaskyPriority>().ToList();
        ViewBag.PrioridadSeleccionada = priority;
        ViewBag.currentAction = "hoy";
        ViewBag.EnableDynamicSubmit = true;
        ViewBag.DefaultComboText = "Todas";

        ViewBag.MenuItems = _configuration.GetSection("MenuItems")
                                            .Get<List<MenuViewModel>>()
                                            .FindAll(m => m.Controller == "Actions");
        ViewBag.Controller = "Actions";
        ViewBag.Action = "Hoy";
        ViewBag.CurrentSection = _configuration.GetSection("MenuItems")
                                                .Get<List<MenuViewModel>>()
                                                .Find(m => m.Controller == "Actions" && m.Action == "Hoy")?
                                                .Name;

        List<TaskyObject> tasks = await _taskManager.GetTasksForToday(priority);

        if (!string.IsNullOrEmpty(filtroInput))
        {
            tasks = tasks.Where(x => x.Subjectt.ToLower().Contains(filtroInput.ToLower()) || x.Sender.ToLower().Contains(filtroInput.ToLower())).ToList();
        }

        if (priority != null)
        {
            tasks = tasks.FindAll(x => x.Priority == priority);
        }

        ViewBag.FiltroInput = filtroInput;
        return View("Index", tasks); 
    }

    public async Task<IActionResult> Spam(TaskyPriority? priority, string filtroInput)
    {
        ViewBag.Prioridades = Enum.GetValues(typeof(TaskyPriority)).Cast<TaskyPriority>().ToList();
        ViewBag.PrioridadSeleccionada = priority;
        ViewBag.currentAction = "spam";
        ViewBag.EnableDynamicSubmit = true;
        ViewBag.DefaultComboText = "Todas";

        ViewBag.MenuItems = _configuration.GetSection("MenuItems")
                                            .Get<List<MenuViewModel>>()
                                            .FindAll(m => m.Controller == "Actions" );
        ViewBag.Controller = "Actions";
        ViewBag.Action = "Spam";
        ViewBag.CurrentSection = _configuration.GetSection("MenuItems")
                                                    .Get<List<MenuViewModel>>()
                                                    .Find(m => m.Controller == "Actions" && m.Action == "Spam")?
                                                    .Name;

        List<TaskyObject> tasks = await _taskManager.GetTasksSpam(priority);

        if (!string.IsNullOrEmpty(filtroInput))
        {
            tasks = tasks.Where(x => x.Subjectt.ToLower().Contains(filtroInput.ToLower()) || x.Sender.ToLower().Contains(filtroInput.ToLower())).ToList();

        }

        if (priority != null)
        {
            tasks = tasks.FindAll(x => x.Priority == priority);
        }

        ViewBag.FiltroInput = filtroInput;
        return View("Index", tasks);
    }

    

    public async Task<IActionResult> Eliminar(string id, string currentAction, TaskyPriority? priority)
    {
        

        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction(currentAction);
        }
        await _taskManager.DeleteTask(id); 
       
        return RedirectToAction(currentAction, new { priority });
    }

    public async Task<IActionResult> Detalle(string id, string currentAction)
    {
        ViewBag.MenuItems = _configuration.GetSection("MenuItems")
                                           .Get<List<MenuViewModel>>()
                                           .FindAll(m => m.Controller == "Actions" );
        ViewBag.Controller = "Actions";
        ViewBag.Action = currentAction;

       


        if (string.IsNullOrEmpty(id))
        {
            return RedirectToAction(currentAction);
        }
        var task = await _taskManager.GetTaskyByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        ViewBag.Prioridades = Enum.GetValues(typeof(TaskyPriority)).Cast<TaskyPriority>().ToList();
        ViewBag.PrioridadSeleccionada = task.Priority;
        ViewBag.StatusSeleccionado = task.Status;
        return View(task);

    }


    [HttpPost]
    public async Task<IActionResult> Editar(TaskyObject task)
    {

        

       
        ViewBag.EnableDynamicSubmit = false;
        await _taskManager.UpdateTask(task);
        return RedirectToAction("Detalle", new { Id = task.IdObject });

    }

    }



 



