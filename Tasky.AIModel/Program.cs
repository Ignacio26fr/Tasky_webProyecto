using Microsoft.ML;
using Tasky.AIModel;

// ---------------------- CREACIÓN Y ENTRENAMIENTO DEL MODELO -------------

string projectDirectory = Directory.GetCurrentDirectory();

string dataPath = Path.Combine(projectDirectory, "..\\..\\..\\TrainingData", "emails.csv");

MLContext mlContext = new MLContext();

var modelTrainer = new ModelTrainer(mlContext);
modelTrainer.TrainModel(dataPath);

Console.WriteLine("Modelo entrenado y guardado.");




// --------------------- CARGA Y USO DEL MODELO --------------------------

string modelPath = Path.Combine(projectDirectory, "..\\..\\..\\Models", "modeloUrgencia.zip");

ITransformer loadedModel = mlContext.Model.Load(modelPath, out var modelSchema);

var predEngine = mlContext.Model.CreatePredictionEngine<InputData, OutputData>(loadedModel);

var inputData = new InputData
{
    Asunto = "Recordatorio del turno medico",
    Mensaje = "Buenos dias le recordamos que tiene un turno medico la proxima semana."
};

var prediction = predEngine.Predict(inputData);




// -------------------- DEPURACIÓN --------------------------------

Console.WriteLine("Detalles de la predicción:");
Console.WriteLine($"Asunto: {inputData.Asunto}");
Console.WriteLine($"Mensaje: {inputData.Mensaje}");

if (!string.IsNullOrEmpty(prediction.Urgencia))
{
    Console.WriteLine($"Predicción de urgencia: {prediction.Urgencia}");
}
else
{
    Console.WriteLine("No se pudo determinar la urgencia.");
}
