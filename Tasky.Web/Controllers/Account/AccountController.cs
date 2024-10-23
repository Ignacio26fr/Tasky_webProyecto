
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Tasky.Logica.Gmail;
using Tasky.Logica.Redis;


namespace Tasky.Web.Controllers.Account;

public class AccountController : Controller
{

    private readonly IGmailAccountService _gmailAccountService;
    private readonly IRedisSessionService _redisSessionService;

    public AccountController(IGmailAccountService gmailAccountService, IRedisSessionService redisSessionService)
    {
        _gmailAccountService = gmailAccountService;
        _redisSessionService = redisSessionService;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult GoogleLogin()
    {
        // Redirige al usuario al flujo de autenticación de Google
        // en redirectUrl le pasamos la acción que se ejecutará después de la autenticación
        var redirectUrl = Url.Action("GoogleResponse", "Account", null, Request.Scheme);

        if(redirectUrl == null)
            return BadRequest();

        var properties = _gmailAccountService.GetGoogleAuthProperties(redirectUrl);
        return _gmailAccountService.Challenge(properties);
    }

    public async Task<IActionResult> GoogleLogout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Actions");
    }

    public async Task<IActionResult> GoogleResponse()
    {

        var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        var googleUser = await _gmailAccountService.GetAccount(authenticateResult);

        if (googleUser == null)
            return BadRequest(); // Error en la autenticación

        //guarddamos los datos del usuario en la session con el token de acceso
     
        await _redisSessionService.SetValueAsync("goo_user_name", googleUser.Name!);
        await _redisSessionService.SetValueAsync("goo_user_email", googleUser.Email!);
        await _redisSessionService.SetValueAsync("goo_user_picture", googleUser.Picture!);
        await _redisSessionService.SetValueAsync("goo_access_token", googleUser.AccessToken!);


        return RedirectToAction("Index", "Actions");
    }
}
