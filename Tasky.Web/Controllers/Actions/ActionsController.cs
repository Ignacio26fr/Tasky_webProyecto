using Microsoft.AspNetCore.Mvc;
using Tasky.Logica.Gmail;
using Tasky.Logica.Redis;


namespace Tasky.Web.Controllers.Actions;

public class ActionsController : Controller
{

    private readonly IGmailTaskyService _gmailTaskyService;
    private readonly IRedisSessionService _redisSessionService;

    public ActionsController(IGmailTaskyService gmailService, IRedisSessionService redisSessionService)
    {
        _gmailTaskyService = gmailService;
        _redisSessionService = redisSessionService;
    }

    public IActionResult Index()
    {
        GetUserInfo();

        if (TempData["emails"] != null)
            ViewBag.Emails = TempData["emails"] as List<EmailInfo>;

        return View();
    }

    //DEBBUG
    public async Task<IActionResult> Inbox()
    {
        //solicitamos datos de session
        var token = await _redisSessionService.GetValueAsync("goo_access_token");
        //solicitamos los correos del inbox
        var emails = _gmailTaskyService.GetInbox(token, 1).Result;
        ViewBag.Emails = emails;
        GetUserInfo();
        return View("Index");

    }

    private void GetUserInfo()
    {
        ViewBag.UserName = _redisSessionService.GetValueAsync("goo_user_name").Result;
        ViewBag.UserEmail = _redisSessionService.GetValueAsync("goo_user_email").Result;
        ViewBag.UserPicture = _redisSessionService.GetValueAsync("goo_user_picture").Result;
        ViewBag.Token = _redisSessionService.GetValueAsync("goo_access_token").Result;
        ViewBag.SuscriptionId = _redisSessionService.GetValueAsync("goo_subscription_id").Result;
    }
}
