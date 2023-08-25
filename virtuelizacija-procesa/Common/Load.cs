using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Load
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double ForecastValue { get; set; }
        public double MeasuredValue { get; set; }
        public double AbsolutePercentageDeviation { get; set; }
        public double SquaredDeviation { get; set; }
        public int ImportedFileId { get; set; }


        public Load()
        {

        }

        public Load(int id, DateTime timeStamp, double forecastValue, double measuredValue,
            double absolutePercentageDeviation, double squaredDeviation, int importedFileId)
        {
            Id = id;
            TimeStamp = timeStamp;
            ForecastValue = forecastValue;
            MeasuredValue = measuredValue;
            AbsolutePercentageDeviation = absolutePercentageDeviation;
            SquaredDeviation = squaredDeviation;
            ImportedFileId = importedFileId;
        }
    }
}
