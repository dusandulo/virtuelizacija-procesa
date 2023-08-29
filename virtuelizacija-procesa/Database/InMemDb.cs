using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class InMemDb
    {
        

        public Dictionary<int, Load> WriteLoad(List<Load> loads, Dictionary<int, Load> dicts)
        {
            int maxId = dicts.Count;
            bool found = false;
            foreach (var x in loads)
            {
                foreach(var y in dicts)
                {
                    if(y.Value.TimeStamp == x.TimeStamp)
                    {
                        if(x.ForecastValue == -1)
                        {
                            y.Value.MeasuredValue = x.MeasuredValue;
                        }
                        else
                        {
                            y.Value.ForecastValue = x.ForecastValue;
                        }
                        found = true;
                    }
                }
                if (!found)
                {
                    x.Id = ++maxId;
                    dicts.Add(x.Id, x);
                    found = false;
                }
            }

            return dicts;
        }
        public Dictionary<int, Audit> WriteAudit(List<Audit> audits, Dictionary<int, Audit> dicts, string path)
        {
            int maxId = dicts.Count;
            foreach (var x in audits)
            {
                x.Id = ++maxId;
                dicts.Add(x.Id, x);
            }
            if(audits.Count == 0)
            {
                Audit y = new Audit(++maxId, DateTime.Now, MessageType.INFO, $"Data from "+ path +" successfully added to database");
                dicts.Add(y.Id, y);
            }

            return dicts;
        }
        public Dictionary<int, ImportedFile> WriteImportedFile(ImportedFile importedFile, Dictionary<int, ImportedFile> dicts)
        {
            int maxId = dicts.Count;
            importedFile.Id = ++maxId;
            dicts.Add(importedFile.Id, importedFile);
            return dicts;
        }
        public List<Load> Read(Dictionary<int, Load> dicts)
        {
            return dicts.Values.ToList();
        }
        public Dictionary<int, Load> WriteCalc(List<Load> loads, Dictionary<int, Load> dicts)
        {
            foreach (var x in loads)
            {
                foreach (var y in dicts)
                {
                    if (y.Value.TimeStamp == x.TimeStamp)
                    {
                        y.Value.AbsolutePercentageDeviation = x.AbsolutePercentageDeviation;
                        y.Value.SquaredDeviation = x.SquaredDeviation;
                    }
                }
            }

            return dicts;
        }
    }

}
