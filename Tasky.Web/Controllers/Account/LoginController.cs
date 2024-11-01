using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tasky.Datos.EF;
using Tasky.Logica;
using System.Security.Claims;
using Tasky.Web.Models;

namespace Tasky.Web.Controllers.Account
{
    public class LoginController : Controller
    {
       
        private readonly SignInManager<AspNetUsers> _signInManager;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly EmailService _emailService;

        public LoginController( SignInManager<AspNetUsers> signInManager, UserManager<AspNetUsers> userManager, EmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "El usuario no existe o el email es incorrecto.");
                    return View("Login", model);
                }


                if (user.EmailConfirmed)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        
                        return RedirectToAction("Index", "Actions");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Fallo el ingreso verifique las credenciales");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Por favor verifica tu correo electrónico.");
                }
            }
            return View("Login", model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            HttpContext.Session.Remove("AccessToken");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

       
       
    }
}
