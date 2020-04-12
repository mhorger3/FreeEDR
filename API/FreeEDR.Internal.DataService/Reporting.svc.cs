using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp;

namespace FreeEDR.Internal.DataService
{
    public class Reporting : IReporting
    {
        public Events GetEvents(string filepath)
        {
            Events r = new Events();
            var text = File.ReadAllText(filepath);
            var actualText = text.Substring(text.IndexOf("\n") + 1);
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Events));

            using (StringReader sr = new StringReader(text))
            {
                r = (Events)ser.Deserialize(sr);
            }
            return r;
        }

        private Events getLocalEvents()
        {
            Events r = new Events();
            var text = File.ReadAllText(@"X:\Github\FreeEDR\API\sysmon-export.xml");
            var actualText = text.Substring(text.IndexOf("\n") + 1);
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Events));

            using (StringReader sr = new StringReader(text))
            {
                r = (Events)ser.Deserialize(sr);
            }
            return r;
        }

        // get a list of all the reports generated from a given time
        public List<String> GetHistoricalReports(DateTime dt)
        {
            List<String> reports = new List<String>();
            string[] filePaths = Directory.GetFiles(@"X:\Github\FreeEDR\API\Files");
            foreach(string x in filePaths)
            {
                // for each string, check to see if the datetime of the report is greater than the given time
                string dateTimeBroke = x.Substring(x.Length - 22, 18);
                DateTime converted = DateTime.ParseExact(dateTimeBroke, "yyyy_dd_M_HH_mm_ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                if(DateTime.Compare(converted, dt) > 0)
                {
                    reports.Add(x);
                }
            }
            return reports;
        }

        public List<Event> GetReport(int name)
        {
            Events r = getLocalEvents();
            Console.WriteLine(r);
            List<Event> returnList = new List<Event>();
            foreach(Event e in r.Event)
            {
                if(e.System.EventID == name.ToString())
                {
                    returnList.Add(e);
                }
            }

            string path = @"X:\Github\FreeEDR\API\Files\report_" + name + "_" + DateTime.Now.ToString("yyyy_dd_M_HH_mm_ss") + ".txt";
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("EventData|Message|Time Created");
                    foreach (Event e in returnList)
                    {
                        foreach(var a in e.EventData.Data)
                        {
                            sw.Write(a.Name + " " + a.Text + "|");
                        }
                        sw.Write(e.RenderingInfo.Message);
                        sw.Write("|" + e.System.TimeCreated);
                        sw.WriteLine("");
                    }
                }
            }

            return returnList;
        }

        // return a list of events given that time
        public List<Event> GetReportDate(int name, DateTime dt)
        {
            throw new NotImplementedException();
        }

        // return a list of events given that time, and a different format
        public List<Event> GetReportDateFormat(int name, DateTime dt, FormatOption f)
        {
            throw new NotImplementedException();
        }

        // return a list of events with a different format
        public List<Event> GetReportFormat(int name, FormatOption f)
        {
            throw new NotImplementedException();
        }

        // get all the different options for reports
        public List<Report> GetReportOptions()
        {
            List<Report> returnList = new List<Report>();
            for(int i = 1; i < 23; i++)
            {
                Report newR = new Report();
                newR.EventID = i;
                switch (i)
                {
                    case 1:
                        newR.name = "Process Creations";
                        break;
                    case 2:
                        newR.name = "Process Changed File Creation Time";
                        break;
                    case 3:
                        newR.name = "Network Connection";
                        break;
                    case 4:
                        newR.name = "Sysmon Service State Changed";
                        break;
                    case 5:
                        newR.name = "Process Terminated";
                        break;
                    case 6:
                        newR.name = "Driver Loaded";
                        break;
                    case 7:
                        newR.name = "Image Loaded";
                        break;
                    case 8:
                        newR.name = "Process Creations";
                        break;
                    case 9:
                        newR.name = "Raw Access Read";
                        break;
                    case 10:
                        newR.name = "Process Access";
                        break;
                    case 11:
                        newR.name = "File Create";
                        break;
                    case 12:
                        newR.name = "Registry Object Create and Delete";
                        break;
                    case 13:
                        newR.name = "Registry Value Set";
                        break;
                    case 14:
                        newR.name = "Registry Key and Value Rename";
                        break;
                    case 15:
                        newR.name = "File Create Stream Hash";
                        break;
                    case 17:
                        newR.name = "Pipe Created";
                        break;
                    case 18:
                        newR.name = "Pipe Connected";
                        break;
                    case 19:
                        newR.name = "WmiEventFilter Activity";
                        break;
                    case 20:
                        newR.name = "WmiEventConsumer Activity";
                        break;
                    case 21:
                        newR.name = "WmiEventConsumerToFilter Activity";
                        break;
                    case 22:
                        newR.name = "DNS Query";
                        break;
                }
                returnList.Add(newR);
            }
            Report newReport = new Report();
            newReport.EventID = 255;
            newReport.name = "Error";
            returnList.Add(newReport);
            return returnList;
        }

        // get a list of reports generated given a range
        public List<string> GetReportRange(int name, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        // export a report via email
        public void ExportReport(int r, string recipient)
        {
            throw new NotImplementedException();
        }

    }
}
