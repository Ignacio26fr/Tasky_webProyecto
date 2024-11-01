using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tasky.Datos.EF;
using Tasky.Logica.Gmail;
using Tasky.Logica.Session;

namespace Tasky.Web.Controllers.Account;

public class GoogleController : Controller
{

    private readonly UserManager<AspNetUsers> _userManager;
    private readonly SignInManager<AspNetUsers> _signInManager;
    private readonly IGoogleAccountService _googleAccountService;
    private readonly IGmailNotificationService _gmailNotificationService;
    private readonly IAcountSessionManager _acountSessionManager;

    public GoogleController(UserManager<AspNetUsers> userManager, 
                            SignInManager<AspNetUsers> signInManager, 
                            IGoogleAccountService googleAccountService, 
                            IGmailNotificationService gmailNotificationService,
                            IAcountSessionManager acountSessionManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _googleAccountService = googleAccountService;
        _gmailNotificationService = gmailNotificationService;
        _acountSessionManager = acountSessionManager;
    }

    [HttpPost]
    public IActionResult SignIn(string returnUrl = null)
    {
        var redirectUrl = Url.Action("GoogleResponse", "Google", null, Request.Scheme);
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
      
        if (!result.Succeeded)
        {
            //TODO: Agregar mensaje de error
            return RedirectToAction("Login");
        }

        var googleAccount = await _googleAccountService.GetGoogleAccountAsync(result);

        var user = await _userManager.FindByEmailAsync(googleAccount.Email);

        if (user == null)
        {

            user = new AspNetUsers
            {
                UserName = googleAccount.Email,
                Email = googleAccount.Email,
                NormalizedEmail = googleAccount.Email!.ToUpper(),
                NormalizedUserName = googleAccount.Email.ToUpper(),
                PhoneNumber = googleAccount.PhoneNumber,
                FirstName = googleAccount.GivenName,
                LastName = googleAccount.Surname,
                imagenDePerfil = googleAccount.Picture,
                EmailConfirmed = true,
                AccessToken = googleAccount.RefreshToken!
            };

            var resultado = await _userManager.CreateAsync(user);
            if (!resultado.Succeeded)
            {
                return RedirectToAction("Login");
            }
        }

        //Todo OK entonces logueo al usuario y suscribo a notificaciones para estar a la escucha de nuevos correos entrantes
        user.AccessToken = googleAccount.RefreshToken!;
        await _userManager.UpdateAsync(user);
        await _signInManager.SignInAsync(user, isPersistent: false);
        var session = await _acountSessionManager.RegisterSession(user, googleAccount.AccessToken);
        await _gmailNotificationService.SubscribeToNotificationsAsync(session);
        return RedirectToAction("Index", "Actions");
    }



}
