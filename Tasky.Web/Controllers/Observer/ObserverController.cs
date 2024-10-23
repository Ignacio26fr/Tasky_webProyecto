
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Tasky.Entidad.GmailAccount.PubSub;
using Tasky.Logica.Gmail;
using Tasky.Logica.Redis;


namespace Tasky.Web.Controllers.Observer;

public class ObserverController : Controller
{
    private readonly IGmailNotificationService _gmailNotificationService;
    private readonly IRedisSessionService _redisSessionService; 
    private readonly IMemoryCache _memoryCache;

    public ObserverController(IGmailNotificationService gmailNotificationService, IRedisSessionService redisSessionService, IMemoryCache memoryCache)
    {
        _gmailNotificationService = gmailNotificationService;
        _redisSessionService = redisSessionService;
        _memoryCache = memoryCache;
    }

    [HttpGet]
    public async Task<IActionResult> SubscribeToGmailNotifications()
    {
        var accessToken = await _redisSessionService.GetValueAsync("goo_access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("No se encontró el token de acceso.");
        }

        await _gmailNotificationService.SubscribeToNotifications(accessToken);

        return Ok("Suscrito a las notificaciones de Gmail.");
    }



    public async Task<IActionResult> UnsubscribeFromGmailNotifications(string subscriptionId)
    {
        var accessToken = await _redisSessionService.GetValueAsync("goo_access_token");
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("No se encontró el token de acceso.");
        }

        await _gmailNotificationService.UnsubscribeFromNotifications(accessToken, subscriptionId);

        return Ok("Desuscrito de las notificaciones de Gmail.");
    }


    [HttpPost]
    public async Task<IActionResult> ReceiveNotification([FromBody] PubSubNotification notification)
    {


        string accessToken;

        // Intentar obtener el accessToken de la caché en memoria
        if (!_memoryCache.TryGetValue("goo_access_token", out accessToken))
        {
            // Si no está en caché, lo obtenemos de Redis
            accessToken = await _redisSessionService.GetValueAsync("goo_access_token");

            // Almacenar el accessToken en caché por 5 minutos (o el tiempo que creas necesario)
            if (!string.IsNullOrEmpty(accessToken))
            {
                _memoryCache.Set("goo_access_token", accessToken, TimeSpan.FromMinutes(5));
            }
        }

        // Si no hay accessToken, devolver Unauthorized
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("El usuario no ha iniciado sesión o no tiene un token válido.");
        }

        // Agregar la notificación a la cola para procesarla más tarde
        
        _gmailNotificationService.AddNotification(notification);

        // Responder rápido que la notificación fue recibida
        return Ok();
    }



  
}
