using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChadWrapper.Requests;
using ChadWrapper.Responses;
using ChadWrapper.Data.Types;
using ChadWrapper.Data;
using ChadWrapper.Boinc;
using System.Linq;
using ChadWrapper.Discord;

namespace ChadWrapper.Handlers
{
    class AddApp
    {
        public static void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
        {
            string jsonStr = reader.ReadToEnd();
            string APIKey = context.Request.Headers.Get("Authorization");

            if (APIKey == null || APIKey.Length != 64)
            {
                writer.WriteLine(new BaseResponse() { Message = "Invalid API key" }.ToJSON());
                return;
            }

            AddAppRequest request = AddAppRequest.FromJSON(jsonStr);
            if (request == null || !request.IsValid())
            {
                writer.WriteLine(new BaseResponse() { Message = "Invalid request" }.ToJSON());
                return;
            }

            long appID = AddAppToBOINC(request, writer);
            if (appID == -1)
                return;

            writer.WriteLine(new AddAppResponse()
            {
                Success = true,
                Message = "App added successfully.",
                AppID = appID
            }.ToJSON());

            if (Global.AlertsWebhookURL != null)
            {
                Webhook.SendMessage(Global.AlertsWebhookURL, string.Format(
                    "A new app has been created with the codename '{0}'",
                    request.Codename
                ));
            }
        }

        public static long AddAppToBOINC(AddAppRequest request, StreamWriter writer)
        {
            long appID = BoincDatabaseManager.AddApp(request.Codename, request.FriendlyName);
            if (appID == -1)
            {
                writer.WriteLine(new BaseResponse() { Message = "Could not add app info to the BOINC DB." }.ToJSON());
                return -1;
            }
            else if (appID == -2)
            {
                writer.WriteLine(new BaseResponse() { Message = "Codename already exists in the DB." }.ToJSON());
                return -1;
            }

            return appID;
        }
    }
}
