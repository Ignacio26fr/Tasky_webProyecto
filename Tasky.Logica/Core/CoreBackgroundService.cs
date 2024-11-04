using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using Tasky.Datos.EF;
using Tasky.Entidad;
using Tasky.Logica.Gmail;
using Tasky_AIModel;

namespace Tasky.Logica.Core;


public interface ICoreBackgroundService
{
    void SaveHistoryId(AspNetUsers user, ulong historyId);
    void AddEmail(EmailInfo emailInfo);
}


public class CoreBackgroundService: BackgroundService, ICoreBackgroundService
{
    public IEventService<TaskEventsArgs> TaskEventService { get; }
   
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentQueue<EmailInfo> _emailQueue;
    private bool _isProcessing;
    public CoreBackgroundService(IServiceScopeFactory scopeFactory, IEventService<TaskEventsArgs> taskEventService)
    {
        TaskEventService = taskEventService;
        _scopeFactory = scopeFactory;
        _emailQueue = new ConcurrentQueue<EmailInfo>();
    }

    public void AddEmail(EmailInfo emailInfo)
    {
        _emailQueue.Enqueue(emailInfo);
        ProcessQueue();
    }
    private void ProcessQueue()
    {
        if (_isProcessing)
        {
            return;
        }

        _isProcessing = true;

        Task.Run(async () =>
        {
            while (_emailQueue.TryDequeue(out var email))
            {
                try
                {
                    await ProcessEmail(email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error procesando email: {ex.Message}");
                    
                }
            }

            _isProcessing = false;
        });
    }

    public async Task ProcessEmail(EmailInfo email)
    {
        Console.WriteLine("CORREO ENTRANTE");
        Console.WriteLine("================");
        Console.WriteLine($"Correo-id: {email.Id}");
        Console.WriteLine($"Correo-Hid: {email.HistoryId}");
        Console.WriteLine($"Correo-asunto: {email.Subject}");
        Console.WriteLine($"Correo-fecha: {email.Date}");
        Console.WriteLine($"Correo-remite: {email.Sender}");
        Console.WriteLine($"Correo-body: {email.Body}");

        GenerateTaskFromEmail(email);

       
    }

    public void GenerateTaskFromEmail(EmailInfo email)
    {

        var fechaExtractor = new FechaExtractor();
        DateTime fechaPresunta = fechaExtractor.ExtraerFecha(email.Body);

        var iaModelData = new SpamMLModel.ModelInput()
        {
            Asunto = email.Subject,
            Texto = email.Body,
        };

        //resultados para analisis de SPAM
        var isSpam = SpamMLModel.Predict(iaModelData);

        var iaPriorityModelData = new UrgencyMLModel.ModelInput()
        {
            Asunto = email.Subject,
            Texto = email.Body,
        };

        //resultados para analisis de urgencia
        var priority = UrgencyMLModel.Predict(iaPriorityModelData);

        var taskyResult = new TaskyObject()
        {
            IdObject = email.Id,
            Subjectt = email.Subject,
            Sender = email.Sender,
            Body = email.Body,
            Date = email.Date,
            Status = false,
            Spam = isSpam.PredictedLabel == 1,
            Priority = GetTaskyPriority(priority.PredictedLabel),
            UserId = email.UserId,
            ExpectData = fechaPresunta,
        };

        Console.WriteLine("TAREA GENERADA Y CLASIFICADA:");
        Console.WriteLine($"Id: {taskyResult.IdObject}");
        Console.WriteLine($"Asunto: {taskyResult.Subjectt}");
        Console.WriteLine($"Remitente: {taskyResult.Sender}");
        Console.WriteLine($"Cuerpo: {taskyResult.Body}");
        Console.WriteLine($"Fecha: {taskyResult.Date}");
        Console.WriteLine($"Estado: {taskyResult.Status}");
        Console.WriteLine($"Spam: {taskyResult.Spam}");
        Console.WriteLine($"Prioridad: {taskyResult.Priority}");

        TaskEventService.Publish(new TaskEventsArgs(taskyResult));

    }

    private TaskyPriority GetTaskyPriority(string resultModel)
    {
        switch (resultModel)
        {
            case "Poco Urgente":
                return TaskyPriority.Baja;
            case "Medio":
                return TaskyPriority.Media;
            case "Urgente":
                return TaskyPriority.Alta;
            default:
                return TaskyPriority.Baja;
        }


    }

  

    public void SaveHistoryId(AspNetUsers user, ulong historyId)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var taskyContext = scope.ServiceProvider.GetRequiredService<TaskyContext>();
            user.GoogleHistoryId = (long)historyId;
            taskyContext.AspNetUsers.Update(user);
            taskyContext.SaveChanges();
        }
    }

   

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // No necesita hacer nada en este caso, solo escucha eventos.
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
       
        base.Dispose();
    }

   
}
