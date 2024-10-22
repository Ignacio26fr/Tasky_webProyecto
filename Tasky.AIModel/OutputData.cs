using Microsoft.ML.Data;

namespace Tasky.AIModel
{
    public class OutputData
    {
        [ColumnName("PredictedLabel")]
        public string Urgencia { get; set; }
    }

}
