using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Database
{
    public class XmlDb
    {
        private static int ID = 1; //id counter

        public FileHandle OpenFile(string path) //otvaranje fajla
        {

            if (!File.Exists(path))
            {
                string start = "";
                if (path.ToLower().Contains("audit"))
                {
                    start = "STAVKE";
                }
                else
                {
                    start = "rows";
                }
                XDocument xml = new XDocument(new XElement(start));
                xml.Save(path);
            }

            MemoryStream memoryStream = new MemoryStream();

            using (FileStream xml = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                xml.CopyTo(memoryStream);
            }

            memoryStream.Position = 0;
            return new FileHandle(memoryStream, Path.GetFileName(path));
        }

        public List<Load> Read(string path) //citanje fajla i cuvanje u listu
        {
            List<Load> loads = new List<Load>();

            using (FileHandle options = OpenFile(path))
            {
                XmlDocument database = new XmlDocument();
                database.Load(options.MemoryStream);

                string date = DateTime.Now.ToString("yyyy-dd-MM");
                XmlNodeList rows = database.SelectNodes($"//row[TIME_STAMP]");

                foreach (XmlNode row in rows)
                {
                    Load load = new Load(ID++,
                        DateTime.Parse(row.SelectSingleNode("TIME_STAMP").InnerText),
                        double.Parse(row.SelectSingleNode("FORECAST_VALUE").InnerText),
                        double.Parse(row.SelectSingleNode("MEASURED_VALUE").InnerText));
                    loads.Add(load);
                }
                options.Dispose();
            }
            return loads;
        }
        private void WriteAudit(List<Audit> audits, string path) //upis audit tabele
        {
            using (FileHandle options = OpenFile(path))
            {
                XmlDocument database = new XmlDocument();
                database.Load(options.MemoryStream);

                XmlNodeList rows = database.SelectNodes("//STAVKA");
                int maxID = rows.Count;

                foreach (Audit a in audits) //dodavanje u postojecu tabelu
                {
                    a.Id = ++maxID;

                    XmlElement newRow = database.CreateElement("STAVKA");

                    XmlElement idElement = database.CreateElement("ID");
                    idElement.InnerText = a.Id.ToString();

                    XmlElement timeStampElement = database.CreateElement("TIME_STAMP");
                    timeStampElement.InnerText = a.TimeStamp.ToString("yyyy-MM-dd HH:mm");

                    XmlElement messageTypeElement = database.CreateElement("MESSAGE_TYPE");
                    messageTypeElement.InnerText = a.MessageType.ToString();

                    XmlElement messageElement = database.CreateElement("MESSAGE");
                    messageElement.InnerText = a.Message;

                    newRow.AppendChild(idElement);
                    newRow.AppendChild(timeStampElement);
                    newRow.AppendChild(messageTypeElement);
                    newRow.AppendChild(messageElement);

                    XmlElement rootElement = database.DocumentElement;
                    rootElement.AppendChild(newRow);
                    database.Save(path);
                }

                if (audits.Count == 0) //kreiranje za uspesno dodavanje
                {
                    XmlElement newRow = database.CreateElement("STAVKA");

                    XmlElement idElement = database.CreateElement("ID");
                    idElement.InnerText = (++maxID).ToString();

                    XmlElement timeStampElement = database.CreateElement("TIME_STAMP");
                    timeStampElement.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    XmlElement messageTypeElement = database.CreateElement("MESSAGE_TYPE");
                    messageTypeElement.InnerText = "INFO";

                    XmlElement messageElement = database.CreateElement("MESSAGE");
                    messageElement.InnerText = "Data successfully added to database";

                    newRow.AppendChild(idElement);
                    newRow.AppendChild(timeStampElement);
                    newRow.AppendChild(messageTypeElement);
                    newRow.AppendChild(messageElement);

                    XmlElement rootElement = database.DocumentElement;
                    rootElement.AppendChild(newRow);
                    database.Save(path);
                }

                options.Dispose();
            }
        }
        private void WriteLoad(List<Load> loads, string path) //upis load tabele
        {
            using (FileHandle options = OpenFile(path))
            {
                XmlDocument database = new XmlDocument();
                database.Load(options.MemoryStream);

                options.MemoryStream.Position = 0;

                XmlNodeList rows = database.SelectNodes("//row");
                int maxID = rows.Count;

                foreach (Load l in loads)
                {
                    l.Id = ++maxID;

                    XmlNode element = null;

                    try
                    {
                        element = database.SelectSingleNode($"//row[TIME_STAMP = '{l.TimeStamp.ToString("yyyy-MM-dd HH:mm")}']");
                    }
                    catch { }


                    if (element != null) //ako element postoji proverava se da li je measured ili forecast value
                    {
                        if (l.ForecastValue == -1)
                        {
                            element.SelectSingleNode("MEASURED_VALUE").InnerText = l.MeasuredValue.ToString();
                            database.Save(path);
                        }
                        else
                        {
                            element.SelectSingleNode("FORECAST_VALUE").InnerText = l.ForecastValue.ToString();
                            database.Save(path);
                        }
                    }
                    else //upis
                    {
                        XmlElement newRow = database.CreateElement("row");

                        XmlElement idElement = database.CreateElement("ID");
                        idElement.InnerText = l.Id.ToString();

                        XmlElement timeStampElement = database.CreateElement("TIME_STAMP");
                        timeStampElement.InnerText = l.TimeStamp.ToString("yyyy-MM-dd HH:mm");

                        XmlElement measuredValueElement = database.CreateElement("MEASURED_VALUE");
                        measuredValueElement.InnerText = l.MeasuredValue.ToString();

                        XmlElement forecastValueElement = database.CreateElement("FORECAST_VALUE");
                        forecastValueElement.InnerText = l.ForecastValue.ToString();

                        XmlElement absoluteDeviationElement = database.CreateElement("ABSOLUTE_PERCENTAGE_DEVIATION");
                        absoluteDeviationElement.InnerText = l.AbsolutePercentageDeviation.ToString();

                        XmlElement squaredDeviationElement = database.CreateElement("SQUARED_DEVIATION");
                        squaredDeviationElement.InnerText = l.SquaredDeviation.ToString();

                        XmlElement importedFileId = database.CreateElement("IMPORTED_FILE_ID");
                        importedFileId.InnerText = l.ImportedFileId.ToString();

                        newRow.AppendChild(idElement);
                        newRow.AppendChild(timeStampElement);
                        newRow.AppendChild(measuredValueElement);
                        newRow.AppendChild(forecastValueElement);
                        newRow.AppendChild(absoluteDeviationElement);
                        newRow.AppendChild(squaredDeviationElement);
                        newRow.AppendChild(importedFileId);

                        XmlElement rootElement = database.DocumentElement;
                        rootElement.AppendChild(newRow);
                        database.Save(path);
                    }
                }

                options.Dispose();
            }
        }
        private void WriteImportedFile(ImportedFile impo, string path) //upis importedFile tabele
        {
            using (FileHandle options = OpenFile(path))
            {
                XmlDocument database = new XmlDocument();
                database.Load(options.MemoryStream);

                XmlNodeList rows = database.SelectNodes("//row");
                int maxID = rows.Count;

                impo.Id = ++maxID;

                XmlElement newRow = database.CreateElement("row");

                XmlElement idElement = database.CreateElement("ID");
                idElement.InnerText = impo.Id.ToString();

                XmlElement fileNameElement = database.CreateElement("FILE_NAME");
                fileNameElement.InnerText = impo.FileName.ToString();

                newRow.AppendChild(idElement);
                newRow.AppendChild(fileNameElement);

                XmlElement rootElement = database.DocumentElement;
                rootElement.AppendChild(newRow);
                database.Save(path);

                options.Dispose();
            }
        }
        public void WriteCalc(List<Load> loads, string path)
        {
            using (FileHandle options = OpenFile(path))
            {
                XmlDocument database = new XmlDocument();
                database.Load(options.MemoryStream);

                options.MemoryStream.Position = 0;

                XmlNodeList rows = database.SelectNodes("//row");
                foreach (Load l in loads)
                {
                    XmlNode element = null;

                    try
                    {
                        element = database.SelectSingleNode($"//row[TIME_STAMP = '{l.TimeStamp.ToString("yyyy-MM-dd HH:mm")}']");
                    }
                    catch { }

                    if (element != null) // upis proracuna u polja ako polja postoje
                    {
                        element.SelectSingleNode("ABSOLUTE_PERCENTAGE_DEVIATION").InnerText = l.AbsolutePercentageDeviation.ToString();
                        element.SelectSingleNode("SQUARED_DEVIATION").InnerText = l.SquaredDeviation.ToString();
                        database.Save(path);
                    }
                }
                options.Dispose();
            }
        }

        public void Write(List<Load> loads, List<Audit> audits, ImportedFile impo, string loadsPath, string auditsPath, string importedFilesPath) // upis/kreiranje load i audit tabele
        {
            WriteLoad(loads, loadsPath);
            WriteAudit(audits, auditsPath);
            WriteImportedFile(impo, importedFilesPath);
        }

    }
}
