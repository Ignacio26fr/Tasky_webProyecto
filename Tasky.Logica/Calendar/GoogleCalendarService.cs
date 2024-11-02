using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Tasky.Datos.EF;
using Tasky.Logica.Session;

namespace Tasky.Logica.Calendar
{
    public interface IGoogleCalendarService
    {
        Task CreateEventAsync(EventosCalendar googleEvent);
    }

    public class GoogleCalendarService : IGoogleCalendarService
    {
        private CalendarService _calendarService;
        private readonly IAcountSessionManager _acountSessionManager;

        public GoogleCalendarService(IAcountSessionManager acountSessionManager)
        {
            _acountSessionManager = acountSessionManager;

        }

        private async Task AuthemticateAsync()
        {
            var session = await _acountSessionManager.GetSession();
            if (session == null)
                throw new Exception("No hay sesion activa");

            var credential = GoogleCredential.FromAccessToken(session.AccessToken);
            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Tasky"
            });
        }

        public async Task CreateEventAsync(EventosCalendar googleEvent)
        {
            await AuthemticateAsync();
            var calendarEvent = new Event
            {
                Summary = googleEvent.Titulo,
                Location = googleEvent.Localizacion,
                Description = googleEvent.Descripcion,
                Start = new EventDateTime()
                {
                    DateTime = googleEvent.FechaInicio,
                    TimeZone = "America/Argentina"
                },
                End = new EventDateTime()
                {
                    DateTime = googleEvent.FechaFin,
                    TimeZone = "America/Lima"
                }


            };
            try
            {
                var createdEvent = await _calendarService.Events.Insert(calendarEvent, "primary").ExecuteAsync();
                Console.WriteLine("Evento creado: {0}", createdEvent.HtmlLink);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear evento: {0}", ex.Message);
                throw;



            }
        }
    }
}