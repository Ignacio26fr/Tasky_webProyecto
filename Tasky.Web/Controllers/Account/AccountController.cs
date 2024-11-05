using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tasky.Datos.EF;



namespace Tasky.Web.Controllers.Account;

public class AccountController : Controller
{

    private readonly UserManager<AspNetUsers> _userManager;

    public AccountController(UserManager<AspNetUsers> userManager)
    {
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }

 

    [HttpGet]
    public async Task<ActionResult> ConfirmarEmail(string userId, string token)
    {

        Console.WriteLine($"ConfirmarEmail - UserID: {userId}, Token: {token}");

        if (userId == null || token == null)
        {
            
            return RedirectToAction("Index", "Login");
        }

        var usuario = await _userManager.FindByIdAsync(userId);
        if (usuario == null)
        {
            Console.WriteLine("Usuario no encontrado");
            return NotFound();
        }

        var resultado = await _userManager.ConfirmEmailAsync(usuario, token);
        if (resultado.Succeeded)
        {
            return RedirectToAction("Success");
        }
        else
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

    public IActionResult Success()
    {
        return View();
    }


}
