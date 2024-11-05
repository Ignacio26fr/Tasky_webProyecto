using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tasky.Datos.EF;
using Tasky.Logica;
using Tasky.Web.Models;

namespace Tasky.Web.Controllers.Account;

public class RegisterController : Controller
{

    private readonly UserManager<AspNetUsers> _userManager;
    private readonly IEmailService _emailService;

    public RegisterController(UserManager<AspNetUsers> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(RegistroViewModel model)
    {
        if (ModelState.IsValid)
        {

            var existeUserName = await _userManager.FindByEmailAsync(model.Email);
            if (existeUserName != null)
            {
                ModelState.AddModelError(string.Empty, "Este correo ya está en uso.");
                return View(model);
            }

            var identity = new AspNetUsers
            {
                UserName = model.Email,
                PhoneNumber = model.Telefono,
                Email = model.Email,
                NormalizedEmail = model.Email,
                NormalizedUserName = model.Email,
            };

            var resultado = await _userManager.CreateAsync(identity, model.Password);

            if (resultado.Succeeded)
            {
                //Aca podria activar la doble autenticacion
                await _userManager.SetTwoFactorEnabledAsync(identity, false);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(identity);
                var callbackUrl = Url.Action("ConfirmarEmail", "Login", new { userId = identity.Id, token }, Request.Scheme);
                await _emailService.SendConfirmationEmailAsync(model.Email, callbackUrl);

                return RedirectToAction("Verificar","Account");
            }

            ModelState.AddModelError(string.Empty, "Fallo el ingreso verifique las credenciales");


        }
        return View(model);
    }

}
