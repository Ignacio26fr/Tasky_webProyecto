using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Text;
using Tasky.Datos.EF;
using Tasky.Logica;
using Tasky.Web.Models;
using System.Security.Claims;

namespace Tasky.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;
        
        private readonly EmailService _emailService;

        public LoginController(UserManager<AspNetUsers> userManager, SignInManager<AspNetUsers> signInManager, EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            
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
                Console.WriteLine("Entre aca");
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
                var errores = string.Join(", ", resultado.Errors.Select(e => e.Description));
                Console.WriteLine($"Error al confirmar correo: {errores}");
                ModelState.AddModelError(string.Empty, $"Error: {errores}");
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
                var user =  await _userManager.FindByEmailAsync(model.Email);

                if(user == null)
                {
                    ModelState.AddModelError(string.Empty, "El usuario no existe o el email es incorrecto.");
                    return View("Login", model);
                }

                
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

        [HttpPost]
        public IActionResult IniciarConGoogle(string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Login", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var claimsIdentity = result.Principal.Identity as ClaimsIdentity;

            
            var email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
            var phone = claimsIdentity?.FindFirst(ClaimTypes.MobilePhone)?.Value;

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                
                user = new AspNetUsers
                {
                    UserName = email,
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper(),
                    PhoneNumber = phone,

                    
                };

                var resultado = await _userManager.CreateAsync(user);
                if (!resultado.Succeeded)
                {
                    return RedirectToAction("Login");
                }
            }

            
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl ?? "/Home");
        }



        private async Task EnviarCorreoAsync(string email, string asunto, string mensj)
        {
            await _emailService.EnviarEmailAsync(email, "Verificación de cuenta", mensj);
        }
    }
}
