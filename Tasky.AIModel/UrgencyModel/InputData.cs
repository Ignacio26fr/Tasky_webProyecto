using Microsoft.ML.Data;

namespace Tasky.AIModel.UrgencyModel
{
    public class InputData
    {
        [LoadColumn(0)]
        public string Asunto { get; set; }
        [LoadColumn(1)]
        public string Mensaje { get; set; }
        [LoadColumn(2)]
        public string Urgencia { get; set; }

    }
}
