using Microsoft.AspNetCore.Mvc;
using Tasky.Logica.Calendar;
using Tasky.Logica.Session;

namespace Tasky.Web.Controllers.Calendar
{
    public class CalendarController : Controller
    {


        private readonly IGoogleCalendarService _googleCalendarService;



        public CalendarController(IGoogleCalendarService googleCalendarService)
        {
            _googleCalendarService = googleCalendarService;

        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var calendarUrl = await _googleCalendarService.RedirectToGoogleCalendar();
                return Json(new { url = calendarUrl });
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return View("Error", new { message = "No se pudo redirigir a Google Calendar." });
            }
        }
    }
}
    

