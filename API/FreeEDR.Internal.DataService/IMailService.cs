﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FreeEDR.Internal.DataService
{
    [ServiceContract]
    public interface IMailService
    {

        // email address for microsoft planner - freeedr@drexel0.onmicrosoft.com
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void SendMail(string sender, string recipient, string subject, string body);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void SendMailAttach(string sender, string recipient, string subject, string body, string attachment);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetMailAttach/?sender={sender}&recipient={recipient}&subject={subject}&body={body}&attachment={attachment}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void GetMailAttach(string sender, string recipient, string subject, string body, string attachment);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetMail/?sender={sender}&recipient={recipient}&subject={subject}&body={body}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void GetMail(string sender, string recipient, string subject, string body);

    }
}
