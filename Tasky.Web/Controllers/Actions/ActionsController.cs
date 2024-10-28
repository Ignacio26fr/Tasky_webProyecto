using Microsoft.AspNetCore.Mvc;
using Tasky.Logica.Gmail;



namespace Tasky.Web.Controllers.Actions;

public class ActionsController : Controller
{

   

    public ActionsController()
    {
        
    }

    public IActionResult Index()
    {
       // GetUserInfo();

        if (TempData["emails"] != null)
            ViewBag.Emails = TempData["emails"] as List<EmailInfo>;

        return View();
    }

    //DEBBUG
    public async Task<IActionResult> Inbox()
    {
        //solicitamos datos de session
      
        //solicitamos los correos del inbox
       // var emails = _gmailTaskyService.GetInbox(token, 1).Result;
      
        return View("Index");

    }


}
