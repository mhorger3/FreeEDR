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
        [WebInvoke(Method = "GET", UriTemplate = "/GetData/?value={value}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetData(int value);

    }

}
