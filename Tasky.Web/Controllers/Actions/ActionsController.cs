using Microsoft.AspNetCore.Mvc;
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

    public IActionResult Index()
    {
        return View();
    }

 


}
