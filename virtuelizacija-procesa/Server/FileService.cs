using Common;
using Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class FileService : IFile
    {
        public static XmlDb database = new XmlDb();
        public static InMemDb inMemDatabase = new InMemDb();
        CalculateDelegate calc = new CalculateDelegate();
        public delegate List<Load> CalculateHandler(object sender, List<Load> args);

        public static Dictionary<int, Load> LoadsBase = new Dictionary<int, Load>();
        public static Dictionary<int, Audit> AuditsBase = new Dictionary<int, Audit>();
        public static Dictionary<int, ImportedFile> ImportedFilesBase = new Dictionary<int, ImportedFile>();


        [OperationBehavior(AutoDisposeParameters = true)]
        public void ParseFile(FileHandle options, bool isForecast) //parsiranje fajla
        {
            List<Audit> errors = new List<Audit>();
            List<Load> values = new List<Load>();

            int line = 1; //brojac redova

            using (StreamReader stream = new StreamReader(options.MemoryStream))
            {
                string data = stream.ReadToEnd();
                string[] csv_rows = data.Split('\n');
                string[] rows = csv_rows.Take(csv_rows.Length - 1).ToArray();

                foreach (var row in rows)
                {
                    char[] parts = { ',' };
                    string[] rowSplit = row.Split(parts, StringSplitOptions.RemoveEmptyEntries);

                    if (csv_rows.Length > 26 || csv_rows.Length < 23) //provera broja redova u datoteci (broj sati u danu)
                    {
                        Audit a = new Audit(0, DateTime.Now, MessageType.ERROR, $"Invalid number of rows in " + options.FileName);
                        errors.Add(a);
                    }
                    else if (rowSplit.Length != 2) //provera formata (da li ima tacno 2 reda)
                    {
                        Audit a = new Audit(0, DateTime.Now, MessageType.ERROR, $"Invalid data format in " + options.FileName);
                        errors.Add(a);
                    }
                    else
                    {
                        if (!DateTime.TryParse(rowSplit[0], out DateTime time)) //provera formata datuma
                        {
                            Audit a = new Audit(0, DateTime.Now, MessageType.ERROR, $"Invalid TimeStamp in " + options.FileName);
                            errors.Add(a);
                        }
                        else
                        {
                            if (!double.TryParse(rowSplit[1], out double value))
                            {
                                Audit a = new Audit(0, DateTime.Now, MessageType.ERROR, $"Invalid Measured Value for date in " + options.FileName);
                                errors.Add(a);
                            }
                            else
                            {
                                if (value < 0.0)
                                {
                                    Audit a = new Audit(0, DateTime.Now, MessageType.ERROR, $"Invalid Measured Value for date " + options.FileName);
                                    errors.Add(a);
                                }

                                else
                                {
                                    if (!isForecast) //provera da li se dodaje measured ili forecast
                                    {
                                        Load l = new Load(1, time, -1, value);
                                        values.Add(l);
                                    }
                                    else
                                    {
                                        Load l = new Load(1, time, value, -1);
                                        values.Add(l);
                                    }
                                }
                            }
                        }
                    }
                    line++;
                }
                stream.Dispose();
            }
            ImportedFile impo = new ImportedFile(1, options.FileName);
            if (ConfigurationManager.AppSettings["DATABASE"].Equals("XML"))
            {
                //poziv metode za upis vrednosti u tabele
                database.Write(values, errors, impo, ConfigurationManager.AppSettings["TBL_LOAD"],
                    ConfigurationManager.AppSettings["TBL_AUDIT"],
                    ConfigurationManager.AppSettings["TBL_IMPORTED"]);
                //Calc(); // pokretanje izracunavanja proracuna
            }
            else if (ConfigurationManager.AppSettings["DATABASE"].Equals("INMEM"))
            {
                LoadsBase = inMemDatabase.WriteLoad(values, LoadsBase);
                AuditsBase = inMemDatabase.WriteAudit(errors, AuditsBase, options.FileName);
                ImportedFilesBase = inMemDatabase.WriteImportedFile(impo, ImportedFilesBase);

                CalcInMemory();
                /*foreach (var x in LoadsBase)
                {
                    Console.WriteLine(x.Key);
                    Console.WriteLine(x.Value.ForecastValue);
                    Console.WriteLine(x.Value.MeasuredValue);
                    Console.WriteLine(x.Value.TimeStamp);
                }*/

                foreach(var x in AuditsBase)
                {
                    Console.WriteLine(x.Value.Message);
                }
                foreach(var x in ImportedFilesBase)
                {
                    Console.WriteLine(x.Value.FileName);
                }
                /*foreach (var x in LoadsBase)
                {
                    Console.WriteLine(x.Key);
                    Console.WriteLine(x.Value.ForecastValue);
                    Console.WriteLine(x.Value.MeasuredValue);
                    Console.WriteLine(x.Value.AbsolutePercentageDeviation);
                    Console.WriteLine(x.Value.SquaredDeviation);
                }*/
            }
            else
            {
                Console.WriteLine("The database type is not set");
            }
        }

        public void Calc()
        {
            List<Load> loads = new List<Load>();
            loads = database.Read(ConfigurationManager.AppSettings["TBL_LOAD"]); // citanje load
            loads = calc.InvokeEvent(loads);

            database.WriteCalc(loads, ConfigurationManager.AppSettings["TBL_LOAD"]); // upis load
        }
        public void CalcInMemory()
        {
            List<Load> loads = new List<Load>();
            loads = inMemDatabase.Read(LoadsBase);
            loads = calc.InvokeEvent(loads);

            LoadsBase = inMemDatabase.WriteCalc(loads, LoadsBase);
        }


    }
}
