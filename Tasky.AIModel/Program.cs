using Microsoft.ML;
using Tasky.AIModel.UrgencyModel;
using Tasky_AIModel;

// -------------------------------- MODELO DE URGENCIA --------------------------------
// ---------------------- CREACIÓN Y ENTRENAMIENTO DEL MODELO ----------------------

string projectDirectory = Directory.GetCurrentDirectory();

string dataPath = Path.Combine(projectDirectory, "..\\..\\..\\TrainingData", "emails.csv");

MLContext mlContext = new MLContext();

var modelTrainer = new ModelTrainer(mlContext);
modelTrainer.TrainModel(dataPath);

Console.WriteLine("Modelo entrenado y guardado.");
Console.WriteLine("\n\n");



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



// -------------------- DEPURACIÓN URGENCIA --------------------------------

Console.WriteLine("Detalles de la predicción de urgencia:");
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







// ---------------------- MODELO SPAM ---------------------------------

// Crear datos de prueba
var sampleData = new SpamMLModel.ModelInput()
{
    Asunto = @"Reunión de proyecto programada para mañana",
    Texto = @"Recuerda que tenemos una reunión importante para discutir los avances del proyecto mañana a las 10 AM.",
};

//Cargar el modelo y hacer predicción
var result = SpamMLModel.Predict(sampleData);

// Depuración
Console.WriteLine("\n\n\n");
Console.WriteLine("Detalles de la predicción de spam:");
Console.WriteLine($"{sampleData.Asunto}");
Console.WriteLine($"{sampleData.Texto}");
Console.WriteLine($"Predicción de spam: {(result.PredictedLabel == 1 ? "Spam" : "No es Spam")}"); // Si PredictedLabe es 1, es spam, si es 0, no es spam


// Esto solo es con fines de medir la precisión, no es necesario.
Console.WriteLine("\nPorcentaje de las probabilidades:");
var sortedScoresWithLabel = SpamMLModel.PredictAllLabels(sampleData);
Console.WriteLine($"{"Class",-40}{"Score",-20}");
Console.WriteLine($"{"-----",-40}{"-----",-20}");

foreach (var score in sortedScoresWithLabel)
{
    Console.WriteLine($"{score.Key,-40}{score.Value,-20}");
}
