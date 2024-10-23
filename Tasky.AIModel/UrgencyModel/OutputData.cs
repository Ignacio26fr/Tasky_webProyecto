using Microsoft.ML.Data;

namespace Tasky.AIModel.UrgencyModel
{
    public class OutputData
    {
        [ColumnName("PredictedLabel")]
        public string Urgencia { get; set; }
    }

}
