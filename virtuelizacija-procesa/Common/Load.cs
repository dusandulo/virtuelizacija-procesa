using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Load
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public double ForecastValue { get; set; }
        [DataMember]
        public double MeasuredValue { get; set; }
        [DataMember]
        public double AbsolutePercentageDeviation { get; set; }
        [DataMember]
        public double SquaredDeviation { get; set; }
        [DataMember]
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

        public Load(int id, DateTime timeStamp, double forecastValue, double measuredValue)
        {
            Id = id;
            TimeStamp = timeStamp;
            ForecastValue = forecastValue;
            MeasuredValue = measuredValue;
        }
    }
}
