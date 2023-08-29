using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Server.FileService;

namespace Server
{
    public class CalculateDelegate
    {
        public event CalculateHandler absEvent;
        public event CalculateHandler powEvent;

        public CalculateDelegate()
        {
            absEvent += CalculateAbsEvent;
            powEvent += CalculateSquareEvent;
        }

        private List<Load> CalculateSquareEvent(object sender, List<Load> loads) // racunanje apsolutnog procentualnog odstupanja
        {
            foreach (Load x in loads)
            {
                x.SquaredDeviation = Math.Pow(((x.MeasuredValue - x.ForecastValue) / x.ForecastValue), 2);
            }
            return loads;
        }
        private List<Load> CalculateAbsEvent(object sender, List<Load> loads) // racunanje kvadratnog odstupanja
        {
            foreach (Load x in loads)
            {
                x.AbsolutePercentageDeviation = ((x.MeasuredValue - x.ForecastValue) / x.ForecastValue) * 100;
            }
            return loads;
        }
        public List<Load> InvokeEvent(List<Load> data)
        {
            List<Load> results = new List<Load>();
            if (ConfigurationManager.AppSettings["Type"] == "abs")
            {
                results = absEvent.Invoke(this, data);
            }
            else
            {
                results = powEvent.Invoke(this, data);
            }
            return results;
        }
    }
}
