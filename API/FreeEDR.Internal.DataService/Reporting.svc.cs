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

namespace FreeEDR.Internal.DataService
{
    public class Reporting : IReporting
    {
        public void ExportReport(Report r, string recipient)
        {
            throw new NotImplementedException();
        }

        public Events GetEvents(string filepath)
        {
            Events r = new Events();
            string text = System.IO.File.ReadAllText(filepath);
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Events));

            using (StringReader sr = new StringReader(text))
            {
                r = (Events)ser.Deserialize(sr);
            }

            return r;
        }

        public List<Report> GetHistoricalReports(DateTime dt)
        {
            throw new NotImplementedException();
        }

        public Report GetReport(string name)
        {
            throw new NotImplementedException();
        }

        public Report GetReportDate(string name, DateTime dt)
        {
            throw new NotImplementedException();
        }

        public Report GetReportDateFormat(string name, DateTime dt, FormatOption f)
        {
            throw new NotImplementedException();
        }

        public Report GetReportFormat(string name, FormatOption f)
        {
            throw new NotImplementedException();
        }

        public List<Report> GetReportOptions()
        {
            throw new NotImplementedException();
        }

        public List<string> GetReportRange(string name, DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}
