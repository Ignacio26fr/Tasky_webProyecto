using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Tasky.Datos.EF;
using Tasky.Web.Models;

namespace Tasky.Web.Controllers.Perfil
{
    public class PerfilController : Controller
    {

        private readonly UserManager<AspNetUsers> _userManager;
        private readonly IConfiguration _configuration;

        public PerfilController(UserManager<AspNetUsers> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            //necesario para menu
            ViewBag.MenuItems = _configuration.GetSection("MenuItems")
                                                .Get<List<MenuViewModel>>()
                                                .FindAll(m => m.Controller == "Perfil");
            ViewBag.Controller = "Perfil";
            ViewBag.Action = "Index";

            //quiero obtener el usuario que esta en session
            var user = _userManager.GetUserAsync(User).Result;
            Console.WriteLine($"El usuario es : {user}");
            if (user == null)
                return RedirectToAction("Index", "Login");
            return View(user);
        }

        public IActionResult ActualizarPerfil()
        {
            //necesario para menu
            ViewBag.MenuItems = _configuration.GetSection("MenuItems")
                                                .Get<List<MenuViewModel>>()
                                                .FindAll(m => m.Controller == "Perfil");
            ViewBag.Controller = "Perfil";
            ViewBag.Action = "ActualizarPerfil";
            var user = _userManager.GetUserAsync(User).Result;
           
            if (user == null)
                return RedirectToAction("Index", "Login");
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarPerfil(string firstName, string lastName, string phoneNumber, IFormFile foto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index", "Login");

            if (string.IsNullOrWhiteSpace(firstName))
            {
                TempData["Error"] = "El nombre no puede estar vacío.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                TempData["Error"] = "El apellido no puede estar vacío.";
                return RedirectToAction("Index");
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;

            if (foto != null && foto.Length > 0)
            {
                var formatosPermitidos = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(foto.FileName);

                if (!formatosPermitidos.Contains(extension))
                {
                    TempData["Error"] = "El formato de la imagen no es válido";
                    return RedirectToAction("Index");
                }

                var guardarEnCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Perfil");
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var ruta = Path.Combine(guardarEnCarpeta, nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                user.imagenDePerfil = nombreArchivo;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = "Foto de perfil actualizada con éxito";
            }
            return RedirectToAction("Index");



            }



    }
}
