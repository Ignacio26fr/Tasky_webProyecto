using Tasky.Entidad.Actions;
using Tasky.Logica.Gmail;
using Tasky_AIModel;
namespace Tasky.Logica.Core;

public interface ITaskManager
{
    void GenerateTaskFromEmail(EmailInfo email);

}

public class TaskManager : ITaskManager
{

    public void GenerateTaskFromEmail(EmailInfo email)
    {

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

        var TaskyResult = new TaskyObject()
        {
            Id = email.Id,
            Subject = email.Subject,
            Sender = email.Sender,
            Body = email.Body,
            Date = email.Date,
            Status = TaskyStatus.Pendiente,
            Spam = isSpam.PredictedLabel == 1,
            Priority = GetTaskyPriority(priority.PredictedLabel)
        };

        Console.WriteLine("TAREA GENERADA Y CLASIFICADA:");
        Console.WriteLine($"Id: {TaskyResult.Id}");
        Console.WriteLine($"Asunto: {TaskyResult.Subject}");
        Console.WriteLine($"Remitente: {TaskyResult.Sender}");
        Console.WriteLine($"Cuerpo: {TaskyResult.Body}");
        Console.WriteLine($"Fecha: {TaskyResult.Date}");
        Console.WriteLine($"Estado: {TaskyResult.Status}");
        Console.WriteLine($"Spam: {TaskyResult.Spam}");
        Console.WriteLine($"Prioridad: {TaskyResult.Priority}");

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
                return TaskyPriority.Urgente;
            default:
                return TaskyPriority.Baja;
        }


    }
}