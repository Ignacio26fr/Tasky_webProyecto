using Microsoft.ML;

namespace Tasky.AIModel
{
    public class ModelTrainer
    {
        private readonly MLContext _mlContext;

        public ModelTrainer(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        public void TrainModel(string dataPath)
        {
            IDataView trainingData = _mlContext.Data.LoadFromTextFile<InputData>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ',');

            var dataEnumerable = _mlContext.Data.CreateEnumerable<InputData>(trainingData, reuseRowObject: false)
                                    .Select(x => new InputData
                                    {
                                        Asunto = x.Asunto,
                                        Mensaje = x.Mensaje,
                                        Urgencia = x.Urgencia.Trim('"')
                                    });

            trainingData = _mlContext.Data.LoadFromEnumerable(dataEnumerable);

            var labelColumn = _mlContext.Transforms.Conversion.MapValueToKey(
                inputColumnName: nameof(InputData.Urgencia),
                outputColumnName: nameof(InputData.Urgencia) + "Key");

            var dataProcessPipeline = _mlContext.Transforms.Text.FeaturizeText(
                    inputColumnName: nameof(InputData.Asunto),
                    outputColumnName: "SubjectFeaturized")
                .Append(_mlContext.Transforms.Text.FeaturizeText(
                    inputColumnName: nameof(InputData.Mensaje),
                    outputColumnName: "BodyFeaturized"))
                .Append(labelColumn)
                .Append(_mlContext.Transforms.Concatenate(
                    "Features", "SubjectFeaturized", "BodyFeaturized"));

            var trainer = _mlContext.MulticlassClassification.Trainers
                .SdcaMaximumEntropy(
                    labelColumnName: nameof(InputData.Urgencia) + "Key",
                    featureColumnName: "Features");

            var trainingPipeline = dataProcessPipeline.Append(trainer)
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var trainedModel = trainingPipeline.Fit(trainingData);

            string projectDirectory = Directory.GetCurrentDirectory();
            string relativePath = Path.Combine(projectDirectory, "..\\..\\..\\Models", "modeloUrgencia.zip"); // Ruta en la que se va a guardar el modelo
            _mlContext.Model.Save(trainedModel, trainingData.Schema, relativePath);
        }

    }
}
