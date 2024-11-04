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
<<<<<<< HEAD
    private readonly IConfiguration _configuration;


    public ActionsController( ITaskManager taskManager, IConfiguration configuration)
    {
        _taskManager = taskManager;
        _configuration = configuration;
=======
    private readonly IGoogleCalendarService _googleCalendarService;


    public ActionsController( ITaskManager taskManager, IGoogleCalendarService googleCalendarService)
    {
        _taskManager = taskManager;
        _googleCalendarService = googleCalendarService;

>>>>>>> Developer
    }

    public async Task<IActionResult> Index(TaskyPriority? idPrioridad, string filtroInput, string filtroSeccion)
    {
<<<<<<< HEAD
        ViewBag.MenuItems = _configuration.GetSection("MenuItems").Get<List<MenuViewModel>>().FindAll(m => m.Controller == "Actions");
        ViewBag.Controller = "Actions";
        ViewBag.Action = "Index";
        return View();
=======

        ViewBag.Prioridades = Enum.GetValues(typeof(TaskyPriority)).Cast<TaskyPriority>().ToList();
        ViewBag.PrioridadSeleccionada = idPrioridad;
        ViewBag.FiltroSeccion = filtroSeccion;

        List<TaskyObject> tasks = await _taskManager.GetTasksForPriority(idPrioridad);

        if(!string.IsNullOrEmpty(filtroInput))
        {
            tasks = tasks.Where(x => x.Subjectt.Contains(filtroInput) || x.Sender.Contains(filtroInput)).ToList();
        }

        return View(tasks);
>>>>>>> Developer
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

    

    public async Task<IActionResult> Eliminar(string id, string filtroSeccion, TaskyPriority? idPrioridad)
    {
        if(string.IsNullOrEmpty(id))
        {
            return RedirectToAction("Index");
        }
        await _taskManager.DeleteTask(id); 
       
        return RedirectToAction("Index", new { filtroSeccion, idPrioridad });
    }

    public async Task<IActionResult> Detalle(string id)
    {
        if(string.IsNullOrEmpty(id))
        {
            return RedirectToAction("Index");
        }
        var task = await _taskManager.GetTaskyByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        return View(task);

    }

    public async Task<IActionResult> Editar(string id)
    {

        if (string.IsNullOrEmpty(id)) {

            return RedirectToAction("Index");
        }

        var task = await _taskManager.GetTaskyByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        ViewBag.Prioridades = Enum.GetValues(typeof(TaskyPriority)).Cast<TaskyPriority>().ToList();
        ViewBag.PrioridadSeleccionada = task.Priority;
        ViewBag.IdSaved = id;

        var model = new EditPriorityViewModel
        {
            IdObject = task.IdObject,
            Priority = task.Priority
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(EditPriorityViewModel task)
    {

        if (!ModelState.IsValid || ViewBag.IdSaved != task.IdObject)
        {
            ViewBag.Prioridades = Enum.GetValues(typeof(TaskyPriority)).Cast<TaskyPriority>().ToList();
            return View(task);
        }
        var tasky = await _taskManager.GetTaskyByIdAsync(task.IdObject);
        if (tasky == null)
        {
            return NotFound();
        }
        tasky.Priority = task.Priority;
        await _taskManager.UpdateTask(tasky);
        return RedirectToAction("Detalle", new { id = task.IdObject });

    }

    }



 



