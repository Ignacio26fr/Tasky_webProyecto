
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Tasky.Datos.EF;
using Tasky.Entidad.GmailAccount.PubSub;
using Tasky.Logica.Core;
using Tasky.Logica.Gmail;
using Tasky.Logica.Session;



namespace Tasky.Web.Controllers.Observer;

public class ObserverController : Controller
{
    private readonly UserManager<AspNetUsers> _userManager;
    private readonly IGmailNotificationService _gmailNotificationService;
    private readonly IMemoryCache _memoryCache;
    private readonly IAcountSessionManager _acountSessionManager;
    private static GoogleSession activeSession = new GoogleSession();
    

    public ObserverController(IGmailNotificationService gmailNotificationService, 
                                UserManager<AspNetUsers> userManager,  
                                IMemoryCache memoryCache,
                                IAcountSessionManager session)
    {
        _userManager = userManager;
        _gmailNotificationService = gmailNotificationService;
        _memoryCache = memoryCache;
        _acountSessionManager = session;
        
    }



    [HttpPost]
    public async Task<IActionResult> ReceiveNotification([FromBody] PubSubNotification notification)
    {


       

        // Intentar obtener el usuario de la caché en memoria
        if (!_memoryCache.TryGetValue("session", out activeSession ))
        {
            // Si no está en caché, consultamos la sesion iniciada
            activeSession = await _acountSessionManager.GetSession();


            // Almacenar el usuario en caché por 5 minutos (o el tiempo que creas necesario)
            if (activeSession != null)
            {
                _memoryCache.Set("session", activeSession , TimeSpan.FromMinutes(5));
            }
        }

        // Si no hay usuario autenticado, devolver Unauthorized
        if (activeSession == null)
        {
            return Unauthorized("El usuario no ha iniciado sesión o no tiene un token válido.");
        }

        // Agregar la notificación a la cola para procesarla más tarde
        _gmailNotificationService.CurrenSession =  activeSession;
        _gmailNotificationService.AddNotification(notification);

        // Responder rápido que la notificación fue recibida
        return Ok();
    }



  
}
