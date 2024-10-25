using Microsoft.ML;
using Tasky_AIModel;


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






// ---------------------- MODELO URGENCIA  ---------------------------------

var sampleDataUrgency = new UrgencyMLModel.ModelInput()
{
    Asunto = @"Turno para transplante de riñon semana que viene",
    Texto = @"Porfavor recuerde que la semana que viene tiene un turno para su transplante de riñon.",
};

//Load model and predict output
var resultUrgency = UrgencyMLModel.Predict(sampleDataUrgency);

// Depuración
Console.WriteLine("\n\n\n");
Console.WriteLine("Detalles de la predicción de urgencia 2:");
Console.WriteLine($"{sampleDataUrgency.Asunto}");
Console.WriteLine($"{sampleDataUrgency.Texto}");
Console.WriteLine($"Predicción de urgencia: {(resultUrgency.PredictedLabel)}");


// Esto solo es con fines de medir la precisión, no es necesario.
Console.WriteLine("\nPorcentaje de las probabilidades:");
var sortedScoresWithLabelUrgency = UrgencyMLModel.PredictAllLabels(sampleDataUrgency);
Console.WriteLine($"{"Class",-40}{"Score",-20}");
Console.WriteLine($"{"-----",-40}{"-----",-20}");

foreach (var score in sortedScoresWithLabelUrgency)
{
    Console.WriteLine($"{score.Key,-40}{score.Value,-20}");
}