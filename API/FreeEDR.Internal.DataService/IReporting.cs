using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Serialization;

namespace FreeEDR.Internal.DataService
{
    [ServiceContract]
    public interface IReporting
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetEvents/?filepath={filepath}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Events GetEvents(string filepath);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetReportOptions", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Report> GetReportOptions();

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<String> GetHistoricalReports(DateTime dt);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Event> GetReport(int name);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Event> GetReportDate(int name, DateTime dt);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Event> GetReportFormat(int name, FormatOption f);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Event> GetReportDateFormat(int name, DateTime dt, FormatOption f);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<string> GetReportRange(int name, DateTime start, DateTime end);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void ExportReport(int r, string recipient);

    }

    [DataContract]
    [XmlRoot(ElementName = "Data", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
    public class Data
    {
        [DataMember]
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [DataMember]
        [XmlText]
        public string Text { get; set; }
    }

    [DataContract]
    [XmlRoot(ElementName = "Event", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
    public class Event
    {
        [DataMember]
        [XmlElement(ElementName = "EventData", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public EventData EventData { get; set; }
        [DataMember]
        [XmlElement(ElementName = "RenderingInfo", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public RenderingInfo RenderingInfo { get; set; }
        [DataMember]
        [XmlElement(ElementName = "System", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public System1 System { get; set; }
        [DataMember]
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [DataContract]
    [XmlRoot(ElementName = "EventData", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
    public class EventData
    {
        [DataMember]
        [XmlElement(ElementName = "Data", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public List<Data> Data { get; set; }
    }

    [DataContract]
    [XmlRoot(ElementName = "Events")]
    public class Events
    {
        [DataMember]
        [XmlElement(ElementName = "Event", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public List<Event> Event { get; set; }
    }

    [DataContract]
    [XmlRoot(ElementName = "Execution", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
    public class Execution
    {
        [DataMember]
        [XmlAttribute(AttributeName = "ProcessID")]
        public string ProcessID { get; set; }
        [DataMember]
        [XmlAttribute(AttributeName = "ThreadID")]
        public string ThreadID { get; set; }
    }

    [DataContract]
    [XmlRoot(ElementName = "Provider", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
    public class Provider
    {
        [DataMember]
        [XmlAttribute(AttributeName = "Guid")]
        public string Guid { get; set; }
        [DataMember]
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }

    [DataContract]
    [XmlRoot(ElementName = "RenderingInfo", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
    public class RenderingInfo
    {
        [DataMember]
        [XmlElement(ElementName = "Channel", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Channel { get; set; }
        [DataMember]
        [XmlAttribute(AttributeName = "Culture")]
        public string Culture { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Keywords", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Keywords { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Level", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Level { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Message", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Message { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Opcode", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Opcode { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Provider", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Provider { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Task", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Task { get; set; }
    }

    [DataContract]
    [XmlRoot(ElementName = "Security", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
    public class Security
    {
        [DataMember]
        [XmlAttribute(AttributeName = "UserID")]
        public string UserID { get; set; }
    }

    [DataContract]
    [XmlRoot(ElementName = "System", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
    public class System1
    {
        [DataMember]
        [XmlElement(ElementName = "Channel", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Channel { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Computer", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Computer { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Correlation", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Correlation { get; set; }
        [DataMember]
        [XmlElement(ElementName = "EventID", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string EventID { get; set; }
        [DataMember]
        [XmlElement(ElementName = "EventRecordID", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string EventRecordID { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Execution", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public Execution Execution { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Keywords", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Keywords { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Level", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Level { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Opcode", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Opcode { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Provider", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public Provider Provider { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Security", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public Security Security { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Task", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Task { get; set; }
        [DataMember]
        [XmlElement(ElementName = "TimeCreated", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public TimeCreated TimeCreated { get; set; }
        [DataMember]
        [XmlElement(ElementName = "Version", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
        public string Version { get; set; }
    }

    [DataContract]
    [XmlRoot(ElementName = "TimeCreated", Namespace = "http://schemas.microsoft.com/win/2004/08/events/event")]
    public class TimeCreated
    {
        [DataMember]
        [XmlAttribute(AttributeName = "SystemTime")]
        public string SystemTime { get; set; }
    }

    [DataContract]
    public class Report
    {  
      // each report is particular to each report ID
      [DataMember]
      public int EventID {get; set;}

      [DataMember]
      public string name { get; set; }
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
