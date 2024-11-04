using Microsoft.AspNetCore.Mvc;
using Tasky.Logica.Core;
using Tasky.Logica.Gmail;
using Tasky.Web.Models;



namespace Tasky.Web.Controllers.Actions;

public class ActionsController : Controller
{

    private readonly ITaskManager _taskManager;
    private readonly IConfiguration _configuration;


    public ActionsController( ITaskManager taskManager, IConfiguration configuration)
    {
        _taskManager = taskManager;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        ViewBag.MenuItems = _configuration.GetSection("MenuItems").Get<List<MenuViewModel>>().FindAll(m => m.Controller == "Actions");
        ViewBag.Controller = "Actions";
        ViewBag.Action = "Index";
        return View();
    }

 


}
