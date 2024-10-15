using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tasky.Datos.EF;
using Tasky.Logica;
using Tasky.Web.Models;

namespace Tasky.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private IUsuarioServicio _usuarioServicio;
        private readonly EmailService _emailService;

        public LoginController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IUsuarioServicio usuarioServicio, EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _usuarioServicio = usuarioServicio;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            
            if (ModelState.IsValid)
            {
               


                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Fallo el ingreso verifique las credenciales");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registro(RegistroViewModel model)
        {
            if(ModelState.IsValid)
            {
                var identity = new Usuario
                {
                   
                    Nombre = model.Nombre,
                    UserName = model.Email,
                    Email = model.Email,
                    NormalizedEmail = model.Email,
                    IdPerfil = 1
                    //Falta agregar el telefono para la dobleauth

                };
                var resultado = await _userManager.CreateAsync(identity, model.Password);

                if (resultado.Succeeded)
                {
                    //Aca podria activar la doble autenticacion
                    await _userManager.SetTwoFactorEnabledAsync(identity, false);
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(identity);
                    var callbackUrl = Url.Action("ConfirmarEmail", "Login",
                new { userId = identity.Id, token = token }, Request.Scheme);


                    await EnviarCorreoAsync(model.Email, "Confirma tu correo",
                $"Por favor confirma tu cuenta haciendo clic aquí: <a href='{callbackUrl}'>enlace</a>");

                    return RedirectToAction("Verificar");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Fallo el ingreso verifique las credenciales");
                }

            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmarEmail(string userId, string token)
        {

            Console.WriteLine($"ConfirmarEmail - UserID: {userId}, Token: {token}");

            if (userId == null || token == null)
            {
                return RedirectToAction("Login", "Index");
            }

            var usuario = await _userManager.FindByIdAsync(userId);
            if(usuario == null)
            {
                Console.WriteLine("Usuario no encontrado");
                return NotFound();
            }

            var resultado = await _userManager.ConfirmEmailAsync(usuario, token);
            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Login");
            } else
            {
                ModelState.AddModelError(string.Empty, "Error al confirmar el correo electrónico.");
                return View("Verificar");
            }
        }

        [HttpGet]
        public IActionResult Verificar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ingresar(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "El usuario no existe o el email es incorrecto.");
                    return View("Login", model);
                }

                Console.WriteLine(user.EmailConfirmed);
                if (user.EmailConfirmed )
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
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
        public async Task<IActionResult> Salir()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }



        private async Task EnviarCorreoAsync(string email, string asunto, string mensj)
        {
            await _emailService.EnviarEmailAsync(email, "Verificación de cuenta", mensj);
        }
    }
}
