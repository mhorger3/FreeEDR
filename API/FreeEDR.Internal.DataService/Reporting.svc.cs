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
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Newtonsoft.Json;
using Spire.Pdf;
using Spire.Pdf.Grid;
using Spire.Pdf.Graphics;
using System.Drawing;

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

        public List<String> GetReports()
        {
            List<String> reports = new List<String>();
            string[] filePaths = Directory.GetFiles(@"X:\Github\FreeEDR\API\Files");
            foreach (string x in filePaths)
            {
               reports.Add(x);
            }
            return reports;
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
                    sw.WriteLine("EventData");
                    foreach (Event e in returnList)
                    {
                        foreach(var a in e.EventData.Data)
                        {
                            sw.Write(a.Name + " " + a.Text + "|");
                        }
                        sw.WriteLine("");
                    }
                }
            }

            return returnList;
        }

        // return a list of events that have happened since that time
        public List<Event> GetReportDate(int name, DateTime dt)
        {
            Events r = getLocalEvents();
            Console.WriteLine(r);
            List<Event> returnList = new List<Event>();
            foreach (Event e in r.Event)
            {
                if (e.System.EventID == name.ToString())
                { // we pull the event
                  // then we need to check for a date
                    string dateTimeBroke = e.System.TimeCreated.SystemTime.ToString();
                    DateTime converted = DateTime.Parse(dateTimeBroke, null,
                                       System.Globalization.DateTimeStyles.RoundtripKind);
                    if(DateTime.Compare(converted, dt) > 0)
                    {
                        returnList.Add(e);
                    }
                }
            }

            string path = @"X:\Github\FreeEDR\API\Files\report_" + name + "_" + DateTime.Now.ToString("yyyy_dd_M_HH_mm_ss") + ".txt";
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("EventData");
                    foreach (Event e in returnList)
                    {
                        foreach (var a in e.EventData.Data)
                        {
                            sw.Write(a.Name + " " + a.Text + "|");
                        }
                        sw.WriteLine("");
                    }
                }
            }

            return returnList;
        }

        // return a list of events given that time, and a different format
        public List<Event> GetReportDateFormat(int name, DateTime dt, FormatOption f)
        {
            Events r = getLocalEvents();
            Console.WriteLine(r);
            List<Event> returnList = new List<Event>();
            foreach (Event e in r.Event)
            {
                if (e.System.EventID == name.ToString())
                {
                    // then we need to check for a date
                    string dateTimeBroke = e.System.TimeCreated.SystemTime.ToString();
                    DateTime converted = DateTime.Parse(dateTimeBroke, null,
                                       System.Globalization.DateTimeStyles.RoundtripKind);
                    if (DateTime.Compare(converted, dt) > 0)
                    {
                        returnList.Add(e);
                    }
                }
            }
            
            // then we either determine which format we want
            if (f == FormatOption.PDF)
            {
                // 1 - UtcTime, OriginalFileName, User
                // 2 - UtcTime, ProcessId, Computer
                // 3 - UtcTime, DestinationIp, ProcessId, Computer
                // 4 - UtcTime, State, Computer
                // 5 - UtcTime, ProcessId, Computer
                // 6 - UtcTime, ImageLoaded, Signature, Computer
                // 7 - UtcTime, ProcessId, Computer
                // 8 - UtcTime, SourceImage, TargetImage, Computer
                // 9 - UtcTime, ProcessId, Computer
                // 10 - UtcTime, ProcessId, Computer
                // 11 - UtcTime, ProcessId, TargetFilename
                // 12 - UtcTime, RuleName, EventType
                // 13 - UtcTime, RuleName, EventType
                // 14 - UtcTime, RuleName, EventType
                // 15 - UtcTime, TargetFileName, Image
                // 17 - UtcTime, ProcessId, Computer
                // 18 - UtcTime, ProcessId, Computer
                // 19 - UtcTime, ProcessId, Computer
                // 20 - UtcTime, ProcessId, Computer
                // 21 - UtcTime, ProcessId, Computer
                // 22 - UtcTime, QueryName, Image, ProcessId


                PdfDocument pdf = new PdfDocument();
                PdfPageBase page = pdf.Pages.Add();
                pdf.PageSettings.Orientation = PdfPageOrientation.Landscape;
                PdfGrid grid = new PdfGrid();
                PdfStringFormat stringFormatCenter = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                PdfStringFormat stringFormatLeft = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                int nestedColumns = 0;


                if(name == 3 || name == 6 || name == 8 || name == 22)
                {
                    nestedColumns = 4;
                } else
                {
                    nestedColumns = 3;
                }

                grid.Columns.Add(nestedColumns);

                PdfGridRow valueRow = grid.Rows.Add(); // and the values go here
                int j = 0; // need to use this for our actual column count
                for (int i = 0; i < returnList[0].EventData.Data.Count; i++)
                {
                    Data a = returnList[0].EventData.Data[i]; // grab the data at the point

                    // need to check if the column name is what we want based on the name
                    switch (name)
                    {
                        case 1:
                            if(a.Name == "UtcTime" || a.Name == "OriginalFileName" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 2:
                            if (a.Name == "UtcTime" || a.Name == "DestinationIp" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 3:
                            if (a.Name == "UtcTime" || a.Name == "DestinationIp" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 187f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 4:
                            if (a.Name == "UtcTime" || a.Name == "State" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 5:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 6:
                            if (a.Name == "UtcTime" || a.Name == "ImageLoaded" || a.Name == "Signature" || a.Name == "Computer")
                            {
                                grid.Columns[j].Width = 187f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 7:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 8:
                            if (a.Name == "UtcTime" || a.Name == "SourceImage" || a.Name == "TargetImage" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 187f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 9:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 10:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 11:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "TargetFilename")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 12:
                            if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 13:
                            if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 14:
                            if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 15:
                            if (a.Name == "UtcTime" || a.Name == "TargetFileName" || a.Name == "Image")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 17:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 18:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 19:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 20:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 21:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 22:
                            if (a.Name == "UtcTime" || a.Name == "QueryName" || a.Name == "Image" || a.Name == "ProcessId")
                            {
                                grid.Columns[j].Width = 187f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                    }
                }

                foreach (Event e in returnList)
                {
                    PdfGridRow nameRow = grid.Rows.Add(); // each embed event has a name
                    int columnPointer = 0;
                    foreach (var a in e.EventData.Data) // so for each event data, add it to the column then
                    {
                        // need to check if the column name is what we want based on the name
                        switch (name)
                        {
                            case 1:
                                if (a.Name == "UtcTime" || a.Name == "OriginalFileName" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 2:
                                if (a.Name == "UtcTime" || a.Name == "DestinationIp" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 3:
                                if (a.Name == "UtcTime" || a.Name == "DestinationIp" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 187f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 4:
                                if (a.Name == "UtcTime" || a.Name == "State" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 5:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 6:
                                if (a.Name == "UtcTime" || a.Name == "ImageLoaded" || a.Name == "Signature" || a.Name == "Computer")
                                {
                                    grid.Columns[columnPointer].Width = 187f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 7:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 8:
                                if (a.Name == "UtcTime" || a.Name == "SourceImage" || a.Name == "TargetImage" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 187f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 9:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 10:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 11:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "TargetFilename")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 12:
                                if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 13:
                                if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 14:
                                if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 15:
                                if (a.Name == "UtcTime" || a.Name == "TargetFileName" || a.Name == "Image")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 17:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 18:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 19:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 20:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 21:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 22:
                                if (a.Name == "UtcTime" || a.Name == "QueryName" || a.Name == "Image" || a.Name == "ProcessId")
                                {
                                    grid.Columns[columnPointer].Width = 187f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                        }
                    }
                }
                grid.Draw(page, 0, 600);
                pdf.Pages.RemoveAt(0); // remove the first blank page
                pdf.SaveToFile(@"X:\Github\FreeEDR\API\Files\report_" + name + "_" + DateTime.Now.ToString("yyyy_dd_M_HH_mm_ss") + ".pdf");
            }
            else if (f == FormatOption.CSV)
            {

                string CSVpath = @"X:\Github\FreeEDR\API\Files\report_" + name + "_" + DateTime.Now.ToString("yyyy_dd_M_HH_mm_ss") + ".csv";
                if (!File.Exists(CSVpath))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(CSVpath))
                    {

                        // first we need to get the a.Name's first
                        Event e0 = returnList[0];
                        for (int i = 0; i < e0.EventData.Data.Count; i++)
                        {
                            Data a = e0.EventData.Data[i]; // grab the data at the point
                            if (i + 1 == e0.EventData.Data.Count)
                            {
                                sw.Write(a.Name);
                            }
                            else
                            {
                                sw.Write(a.Name + ",");
                            }
                        }
                        sw.WriteLine("");
                        foreach (Event e in returnList)
                        {
                            for (int i = 0; i < e.EventData.Data.Count; i++)
                            {
                                Data a = e.EventData.Data[i]; // grab the data at the point
                                if (i + 1 == e.EventData.Data.Count)
                                {
                                    sw.Write(a.Text);
                                }
                                else
                                {
                                    sw.Write(a.Text + ",");
                                }
                            }
                            sw.WriteLine("");
                        }
                    }
                }
            }
            else if (f == FormatOption.TXT)
            {
                string path = @"X:\Github\FreeEDR\API\Files\report_" + name + "_" + DateTime.Now.ToString("yyyy_dd_M_HH_mm_ss") + ".txt";
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        foreach (Event e in returnList)
                        {
                            foreach (var a in e.EventData.Data)
                            {
                                sw.Write(a.Name + " " + a.Text + "|");
                            }
                            sw.WriteLine("");
                        }
                    }
                }
            }
            return returnList;
        }

        // return a list of events with a different format
        public List<Event> GetReportFormat(int name, FormatOption f)
        {
            Events r = getLocalEvents();
            Console.WriteLine(r);
            List<Event> returnList = new List<Event>();
            foreach (Event e in r.Event)
            {
                if (e.System.EventID == name.ToString())
                {
                    returnList.Add(e);
                }
            }

            // then we either determine which format we want
            if (f == FormatOption.PDF)
            {
                PdfDocument pdf = new PdfDocument();
                PdfPageBase page = pdf.Pages.Add();
                pdf.PageSettings.Orientation = PdfPageOrientation.Landscape;
                PdfGrid grid = new PdfGrid();
                PdfStringFormat stringFormatCenter = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                PdfStringFormat stringFormatLeft = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                int nestedColumns = 0;


                if (name == 3 || name == 6 || name == 8 || name == 22)
                {
                    nestedColumns = 4;
                }
                else
                {
                    nestedColumns = 3;
                }

                grid.Columns.Add(nestedColumns);

                PdfGridRow valueRow = grid.Rows.Add(); // and the values go here
                int j = 0; // need to use this for our actual column count
                for (int i = 0; i < returnList[0].EventData.Data.Count; i++)
                {
                    Data a = returnList[0].EventData.Data[i]; // grab the data at the point

                    // need to check if the column name is what we want based on the name
                    switch (name)
                    {
                        case 1:
                            if (a.Name == "UtcTime" || a.Name == "OriginalFileName" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 2:
                            if (a.Name == "UtcTime" || a.Name == "DestinationIp" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 3:
                            if (a.Name == "UtcTime" || a.Name == "DestinationIp" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 187f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 4:
                            if (a.Name == "UtcTime" || a.Name == "State" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 5:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 6:
                            if (a.Name == "UtcTime" || a.Name == "ImageLoaded" || a.Name == "Signature" || a.Name == "Computer")
                            {
                                grid.Columns[j].Width = 187f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 7:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 8:
                            if (a.Name == "UtcTime" || a.Name == "SourceImage" || a.Name == "TargetImage" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 187f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 9:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 10:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 11:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "TargetFilename")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 12:
                            if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 13:
                            if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 14:
                            if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 15:
                            if (a.Name == "UtcTime" || a.Name == "TargetFileName" || a.Name == "Image")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 17:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 18:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 19:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 20:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 21:
                            if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                            {
                                grid.Columns[j].Width = 250f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                        case 22:
                            if (a.Name == "UtcTime" || a.Name == "QueryName" || a.Name == "Image" || a.Name == "ProcessId")
                            {
                                grid.Columns[j].Width = 187f;
                                if (a.Name != null)
                                {
                                    if (a.Name.Length > 30)
                                    {
                                        a.Name = a.Name.Substring(0, 30);
                                    }
                                }
                                valueRow.Cells[j].Value = a.Name; // sets the column names
                                valueRow.Cells[j].StringFormat = stringFormatCenter;
                                j++;
                            }
                            break;
                    }
                }

                foreach (Event e in returnList)
                {
                    PdfGridRow nameRow = grid.Rows.Add(); // each embed event has a name
                    int columnPointer = 0;
                    foreach (var a in e.EventData.Data) // so for each event data, add it to the column then
                    {
                        // need to check if the column name is what we want based on the name
                        switch (name)
                        {
                            case 1:
                                if (a.Name == "UtcTime" || a.Name == "OriginalFileName" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 2:
                                if (a.Name == "UtcTime" || a.Name == "DestinationIp" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 3:
                                if (a.Name == "UtcTime" || a.Name == "DestinationIp" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 187f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 4:
                                if (a.Name == "UtcTime" || a.Name == "State" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 5:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 6:
                                if (a.Name == "UtcTime" || a.Name == "ImageLoaded" || a.Name == "Signature" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 187f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 7:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 8:
                                if (a.Name == "UtcTime" || a.Name == "SourceImage" || a.Name == "TargetImage" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 187f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 9:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 10:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 11:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "TargetFilename")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 12:
                                if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 13:
                                if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 14:
                                if (a.Name == "UtcTime" || a.Name == "RuleName" || a.Name == "EventType")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 15:
                                if (a.Name == "UtcTime" || a.Name == "TargetFileName" || a.Name == "Image")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 17:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 18:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 19:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 20:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 21:
                                if (a.Name == "UtcTime" || a.Name == "ProcessId" || a.Name == "User")
                                {
                                    grid.Columns[columnPointer].Width = 250f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                            case 22:
                                if (a.Name == "UtcTime" || a.Name == "QueryName" || a.Name == "Image" || a.Name == "ProcessId")
                                {
                                    grid.Columns[columnPointer].Width = 187f;
                                    if (a.Text != null)
                                    {
                                        if (a.Text.Length > 30)
                                        {
                                            a.Text = a.Text.Substring(0, 30);
                                        }
                                    }
                                    nameRow.Cells[columnPointer].Value = a.Text;
                                    nameRow.Cells[columnPointer].StringFormat = stringFormatLeft;
                                    columnPointer++;
                                }
                                break;
                        }
                    }
                }
                grid.Draw(page, 0, 600);
                pdf.Pages.RemoveAt(0); // remove the first blank page
                pdf.SaveToFile(@"X:\Github\FreeEDR\API\Files\report_" + name + "_" + DateTime.Now.ToString("yyyy_dd_M_HH_mm_ss") + ".pdf");
            }
            else if (f == FormatOption.CSV)
            {

                string CSVpath = @"X:\Github\FreeEDR\API\Files\report_" + name + "_" + DateTime.Now.ToString("yyyy_dd_M_HH_mm_ss") + ".csv";
                if (!File.Exists(CSVpath))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(CSVpath))
                    {

                        // first we need to get the a.Name's first
                        Event e0 = returnList[0];
                        for (int i = 0; i < e0.EventData.Data.Count; i++)
                        {
                            Data a = e0.EventData.Data[i]; // grab the data at the point
                            if (i + 1 == e0.EventData.Data.Count)
                            {
                                sw.Write(a.Name);
                            }
                            else
                            {
                                sw.Write(a.Name + ",");
                            }
                        }
                        sw.WriteLine("");
                        foreach (Event e in returnList)
                        {
                            for (int i = 0; i < e.EventData.Data.Count; i++)
                            {
                                Data a = e.EventData.Data[i]; // grab the data at the point
                                if (i + 1 == e.EventData.Data.Count)
                                {
                                    sw.Write(a.Text);
                                }
                                else
                                {
                                    sw.Write(a.Text + ",");
                                }
                            }
                            sw.WriteLine("");
                        }
                    }
                }
            }
            else if (f == FormatOption.TXT)
            {
                string path = @"X:\Github\FreeEDR\API\Files\report_" + name + "_" + DateTime.Now.ToString("yyyy_dd_M_HH_mm_ss") + ".txt";
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        foreach (Event e in returnList)
                        {
                            foreach (var a in e.EventData.Data)
                            {
                                sw.Write(a.Name + " " + a.Text + "|");
                            }
                            sw.WriteLine("");
                        }
                    }
                }
            }

            return returnList;
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

        // get a list of reports generated given a range from a start and end time
        public List<string> GetReportRange(int name, DateTime start, DateTime end)
        {
            List<String> reports = new List<String>();
            string[] filePaths = Directory.GetFiles(@"X:\Github\FreeEDR\API\Files");
            foreach (string x in filePaths)
            {
                // for each string, check to see if the datetime of the report is greater than the given time
                string dateTimeBroke = x.Substring(x.Length - 22, 18);
                DateTime converted = DateTime.ParseExact(dateTimeBroke, "yyyy_dd_M_HH_mm_ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                if ((DateTime.Compare(converted, start) > 0) && (DateTime.Compare(converted, end) < 0))
                {
                    string reportName = x.Substring(x.Length - 24, 1);
                    if(reportName == name.ToString())
                    {
                        reports.Add(x);
                    }
                }
            }
            return reports;
        }

        // export a report via email
        public string ExportReport(string path, string recipient)
        {
            AppSettingsReader reader = new AppSettingsReader();
            var endpoint = reader.GetValue("MailService", typeof(string)).ToString();
            var client = new RestClient(endpoint);
            var request = new RestRequest();
            request = new RestRequest("SendMailAttach", Method.POST); // get all the investors
            string body = JsonConvert.SerializeObject(new { sender = "freeEDR@outlook.com", recipient = recipient, subject = "Exported Reports: " + DateTime.Now.ToShortDateString(), body = "Please save your following attachments to view the report. \n -FreeEDR Team", attachment = path });
            request.RequestFormat = DataFormat.Json; // json format
            request.AddJsonBody(body);
            var response = client.Execute(request); // execute the result
            JObject o = JObject.Parse(response.Content); // parse the content
            return response.StatusCode.ToString();
        }

    }
}
