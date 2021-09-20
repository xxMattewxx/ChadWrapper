using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ChadWrapper.Boinc;
using ChadWrapper.Requests;
using ChadWrapper.Responses;
using ChadWrapper.Data.Types;

namespace ChadWrapper.Handlers
{
    class GetProjectInfo
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

            GetProjectRequest request = GetProjectRequest.FromJSON(jsonStr);
            if (request == null || !request.IsValid())
            {
                writer.WriteLine(new BaseResponse() { Message = "Invalid request" }.ToJSON());
                return;
            }

            AppInfo app = AppInfo.GetFromCodename(request.Codename);
            if (app == null)
            {
                writer.WriteLine(new BaseResponse() { Message = "Unable to find app." }.ToJSON());
                return;
            }

            List<BinaryInfo> binaries = BoincDatabaseManager.GetAppBinaries(app.ID);

            writer.WriteLine(new GetProjectResponse()
            {
                Success = true,
                Binaries = binaries,
                FriendlyName = app.FriendlyName
            }.ToJSON());
        }
    }
}
