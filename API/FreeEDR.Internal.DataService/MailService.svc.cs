using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.IO;

namespace FreeEDR.Internal.DataService
{
    public class MailService : IMailService
    {

        public void SendMail(string sender, string recipient, string subject, string body)
        {
            SmtpClient client = new SmtpClient("smtp.live.com");

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sender, ConfigurationManager.AppSettings.Get("credentials").ToString());
            client.EnableSsl = true;
            client.Credentials = credentials;

            try
            {
                var mail = new MailMessage(sender.Trim(), recipient.Trim());
                mail.Subject = subject;
                mail.Body = body;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public void SendMailAttach(string sender, string recipient, string subject, string body, string attachment)
        {
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sender, ConfigurationManager.AppSettings.Get("credentials").ToString());
            client.EnableSsl = true;
            client.Credentials = credentials;

            try
            {
                var mail = new MailMessage(sender.Trim(), recipient.Trim());
                mail.Subject = subject;
                mail.Body = body;
                mail.Attachments.Add(new Attachment(attachment));
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
