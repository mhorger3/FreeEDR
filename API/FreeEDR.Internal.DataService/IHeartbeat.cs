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
    public interface IHeartbeat
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/CheckAlive", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        HeartbeatStatus CheckAlive();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/FetchData", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool FetchData();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/Restart", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void Restart();
    }

    [DataContract]
    public class HeartbeatStatus
    {
        private bool statuscode;
        private int servicesDown;
        private string services;

        [DataMember]
        public bool HeartbeatStatusCode
        {
            get { return statuscode; }
            set { statuscode = value; }
        }

        [DataMember]
        public int HeartbeatDown
        {
            get { return servicesDown; }
            set { servicesDown = value; }
        }

        [DataMember]
        public string HeartbeatServices
        {
            get { return services; }
            set { services = value; }
        }
    }

}
