
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Tasky.Datos.EF;
using Tasky.Entidad.GmailAccount.PubSub;
using Tasky.Logica.Gmail;



namespace Tasky.Web.Controllers.Observer;

public class ObserverController : Controller
{
    private readonly UserManager<AspNetUsers> _userManager;
    private readonly IGmailNotificationService _gmailNotificationService;
    private readonly IMemoryCache _memoryCache;

    public ObserverController(IGmailNotificationService gmailNotificationService, UserManager<AspNetUsers> userManager,  IMemoryCache memoryCache)
    {
        _userManager = userManager;
        _gmailNotificationService = gmailNotificationService;
        _memoryCache = memoryCache;
    }



    [HttpPost]
    public async Task<IActionResult> ReceiveNotification([FromBody] PubSubNotification notification)
    {


        AspNetUsers userAuthenticated;

        // Intentar obtener el usuario de la caché en memoria
        if (!_memoryCache.TryGetValue("user", out userAuthenticated))
        {
            // Si no está en caché, consultamos la sesion iniciada
            var user =  _userManager.GetUserAsync(User).Result;
           

            // Almacenar el usuario en caché por 5 minutos (o el tiempo que creas necesario)
            if (user != null)
            {
                userAuthenticated = user;
                _memoryCache.Set("user", user , TimeSpan.FromMinutes(5));
            }
        }

        // Si no hay usuario autenticado, devolver Unauthorized
        if (userAuthenticated == null)
        {
            return Unauthorized("El usuario no ha iniciado sesión o no tiene un token válido.");
        }

        // Agregar la notificación a la cola para procesarla más tarde
        _gmailNotificationService.CurrenUser = userAuthenticated;
        _gmailNotificationService.AddNotification(notification);

        // Responder rápido que la notificación fue recibida
        return Ok();
    }



  
}
