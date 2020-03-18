using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FreeEDR.Internal.DataService
{
    public class Heartbeat : IHeartbeat
    {
        public HeartbeatStatus CheckAlive()
        {
            // Check to see if we can ping the other two services, obviously if this service is down, this endpoint won't return anything
            HeartbeatStatus check = new HeartbeatStatus();
            string checkServices;
            int serviceDown;
            bool servicesUp;
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            AppSettingsReader reader = new AppSettingsReader();
            System.Net.NetworkInformation.PingReply mailServicePing = ping.Send(reader.GetValue("MailService", typeof(string)).ToString());
            System.Net.NetworkInformation.PingReply reportServicePing = ping.Send(reader.GetValue("ReportingService", typeof(string)).ToString());

            // once we get the pings, check the status of each
            if(mailServicePing.Status == System.Net.NetworkInformation.IPStatus.Success && reportServicePing.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                checkServices = "";
                serviceDown = 0;
                servicesUp = true;
            }
            else if (mailServicePing.Status != System.Net.NetworkInformation.IPStatus.Success && reportServicePing.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                checkServices = "MailService";
                serviceDown = 1;
                servicesUp = false;

            } else if (reportServicePing.Status == System.Net.NetworkInformation.IPStatus.Success && reportServicePing.Status != System.Net.NetworkInformation.IPStatus.Success)
            {
                checkServices = "ReportingService";
                serviceDown = 1;
                servicesUp = false;
            } else
            {
                checkServices = "MailService, ReportingService";
                serviceDown = 2;
                servicesUp = false;
            }
            check.HeartbeatDown = serviceDown;
            check.HeartbeatStatusCode = servicesUp;
            check.HeartbeatServices = checkServices;

            return check;
        }

        public bool FetchData()
        {
            // Need to check the data directory to see if there are any new files after this endpoint is called.
            // Mostly, this should be true unless the Heartbeat endpoint request is lagging behind
            DateTime endpointTime = new DateTime();
            AppSettingsReader reader = new AppSettingsReader();
            DirectoryInfo dir = new DirectoryInfo(reader.GetValue("DataDir", typeof(string)).ToString());
            FileInfo[] files = dir.GetFiles();
            foreach(var file in files)
            {
                // if there are any files that have a greater time, freeze all operations
                if(file.LastWriteTime > endpointTime)
                {
                    return false;
                }
            }
            return true;
        }

        public void Restart()
        {
            // For this endpoint, we need to call iisreset.exe on the server
            // This will completely reboot IIS and all services
            string commandText;
            commandText = "issreset /noforce";
            System.Diagnostics.Process.Start("CMD.exe", commandText);
            return;
        }
        

    }
}
