using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Google.Api;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.DependencyInjection;
using Tasky.Datos.EF;
using Tasky.Logica.Core;
using Tasky.Logica.Session;

namespace Tasky.Logica.Calendar
{
    public interface IGoogleCalendarService
    {
        Task CreateEventAsync(EventosCalendar googleEvent);
        Task<List<EventosCalendar>> GetEventsAsync();
        Task<string> RedirectToGoogleCalendar();
    }

    public class GoogleCalendarService : IGoogleCalendarService, IObserver<TaskEventsArgs>
    {
        
        private readonly IAcountSessionManager _acountSessionManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDisposable _taskSubscription;
        private readonly TaskyContext _context;
        public GoogleCalendarService(IAcountSessionManager acountSessionManager, IServiceScopeFactory scopeFactory, IEventService<TaskEventsArgs> taskEventService, TaskyContext context)
        {
            _acountSessionManager = acountSessionManager;
            _scopeFactory = scopeFactory;
            _taskSubscription = taskEventService.Subscribe(this);
            Console.WriteLine("GoogleCalendarService ha sido instanciado y suscrito.");
            _context = context;
        }
        private async Task<CalendarService> AuthenticateAsync()
        {
            var session = await _acountSessionManager.GetSession();
            if (session == null)
                throw new Exception("No hay sesion activa");

            var credential = GoogleCredential.FromAccessToken(session.AccessToken)
                .CreateScoped(new[] { "https://www.googleapis.com/auth/calendar" });
            return new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Tasky"
            });
        }


        public async Task CreateEventAsync(EventosCalendar googleEvent)
        {
            var calendarService = await AuthenticateAsync();
            var calendarEvent = new Event
            {
                Summary = googleEvent.Titulo,
                Location = googleEvent.Localizacion,
                Description = googleEvent.Descripcion,
                Start = new EventDateTime()
                {
                    DateTimeDateTimeOffset = googleEvent.FechaInicio,
                    TimeZone = "America/Argentina/Buenos_Aires"
                },
                End = new EventDateTime()
                {
                    DateTimeDateTimeOffset = googleEvent.FechaFin,
                    TimeZone = "America/Argentina/Buenos_Aires"
                }


            };
            try
            {
                Console.WriteLine("Creando evento en Google Calendar...");
                var createdEvent = await calendarService.Events.Insert(calendarEvent, "primary").ExecuteAsync();
                Console.WriteLine("Evento creado: {0}", createdEvent.HtmlLink);
                var eventToSave = new EventosCalendar
                {
                    GoogleEventId = createdEvent.Id,
                    Titulo = googleEvent.Titulo,
                    Localizacion = googleEvent.Localizacion,
                    Descripcion = googleEvent.Descripcion,
                    FechaInicio = googleEvent.FechaInicio,
                    FechaFin = googleEvent.FechaFin
                };

                _context.EventosCalendars.Add(eventToSave);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear evento: {0}", ex.Message);
                throw;



            }
        }



        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public async void OnNext(TaskEventsArgs value)
        {
            Console.WriteLine("Entre al onNext");
         try
            {
                var googleEvent = MapToGoogleCalendar(value.TaskObject);
                Console.WriteLine("Evento mapeado: " + googleEvent.Titulo);
                using (var scope = _scopeFactory.CreateScope())
                {
                    var calendarService = scope.ServiceProvider.GetRequiredService<IGoogleCalendarService>();
                    if (calendarService == null)
                    {
                        Console.WriteLine("IGoogleCalendarService no está registrado en el alcance actual.");
                    }
                    else
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IGoogleCalendarService>();
                        await service.CreateEventAsync(googleEvent);
                        Console.WriteLine("Evento creado en Google Calendar");
                    }
                    
                }
            }
            catch(Exception ex) {
                Console.WriteLine("Error al crear evento en Google Calendar: {0}", ex.Message);
                Console.WriteLine("Detalle de la excepción: " + ex.StackTrace);
            }

        }

        public async Task<List<EventosCalendar>> GetEventsAsync()
        {
            var authResult = await _acountSessionManager.GetSession();
            if (authResult == null)
                throw new Exception("No hay sesion activa");
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromAccessToken(authResult.AccessToken),
                ApplicationName = "Tasky"
            });

            var request = service.Events.List("primary");
            request.TimeMinDateTimeOffset = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var googleEvents = await request.ExecuteAsync();
            var eventos = googleEvents.Items.Select(e => new EventosCalendar
            {
                GoogleEventId = e.Id,
                Titulo = e.Summary,
                Descripcion= e.Description,
                FechaInicio = (DateTime)(e.Start?.DateTime),
                FechaFin = (DateTime)(e.End?.DateTime),
            }).ToList();

            return eventos;
        }
        public async Task<string> RedirectToGoogleCalendar()
        {
            var session = await _acountSessionManager.GetSession();
            if (session == null)
                throw new Exception("No hay sesion activa");

            string accessToken = session.AccessToken;

           
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync("https://www.googleapis.com/calendar/v3/calendars/primary/events");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("El token de acceso no es válido o ha expirado.");
            }

            // Si el token es válido, redirige al usuario a Google Calendar
            string url = "https://calendar.google.com/";


     



            return url;
        }

        private EventosCalendar MapToGoogleCalendar(TaskyObject task)
        {
            return new EventosCalendar
            {
                Titulo = task.Subjectt,
                FechaInicio = task.ExpectData,
                FechaFin = task.ExpectData.AddHours(1),


            };
        }

        
    }
}