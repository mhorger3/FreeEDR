using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FreeEDR.Internal.DataService
{
    [ServiceContract]
    public interface IReporting
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetReportOptions", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Report> GetReportOptions();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Report> GetHistoricalReports(DateTime dt);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Report GetReport(string name);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Report GetReportDate(string name, DateTime dt);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Report GetReportFormat(string name, FormatOption f);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Report GetReportDateFormat(string name, DateTime dt, FormatOption f);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> GetReportRange(string name, DateTime start, DateTime end);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void ExportReport(Report r, string recipient);

    }

    [DataContract]
    public class Report
    {
        private string p_name;
        private string p_folder;
        private string p_filepath;
        private DateTime date;
        private FormatOption p_fileFormat;
        // Apply the DataMemberAttribute to the property.
        [DataMember]
        public string Name
        {

            get { return p_name; }
            set { p_name = value; }
        }

        [DataMember]
        public string Folder
        {

            get { return p_folder; }
            set { p_folder = value; }
        }

        [DataMember]
        public string Filepath
        {

            get { return p_filepath; }
            set { p_filepath = value; }
        }

        [DataMember]
        public DateTime ReportDate
        {

            get { return date; }
            set { date = value; }
        }

        [DataMember]
        public FormatOption Format
        {

            get { return p_fileFormat; }
            set { p_fileFormat = value; }
        }
    }

    public enum FormatOption
    {
        PDF,
        TXT,
        CSV,
        DOCX,
        XLS
    }

}
