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
    class AddBinary
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

            AddBinaryRequest request = AddBinaryRequest.FromJSON(jsonStr);
            if (request == null || !request.IsValid())
            {
                writer.WriteLine(new BaseResponse() { Message = "Invalid request" }.ToJSON());
                return;
            }

            if (!CheckPlatformExist(request.Binary, writer))
                return;

            if (!DownloadFile(request.Binary, writer))
                return;

            AppInfo app = AppInfo.GetFromCodename(request.Codename);
            if (app == null)
            {
                writer.WriteLine(new BaseResponse() { Message = "App codename not found." }.ToJSON());
                return;
            }

            if (!AddVersionToBOINC(request, writer, app.ID))
                return;

            writer.WriteLine(new AddBinaryResponse()
            {
                Success = true,
                Message = "App and binaries added successfully.",
                Binary = request.Binary
            }.ToJSON());
            writer.Close();

            if (Global.AlertsWebhookURL == null)
                return;

            Webhook.SendMessage(Global.AlertsWebhookURL, string.Format(
                "New binary has been added to the app '{0}':\n" +
                "Platform: {1} | Version number: {2}\n" +
                "- File URL: {3}\n" +
                "- File Hash: {4}\n" +
                "- File Size: {5}\n",

                request.Codename,
                request.Binary.Platform,
                request.Binary.VersionNumber,
                request.Binary.BinaryURL,
                request.Binary.FileHash,
                request.Binary.FileSize
            ));
        }

        public static bool CheckPlatformExist(BinaryInfo binary, StreamWriter writer)
        {
            if (BoincDatabaseManager.GetPlatformID(binary.Platform) == -1)
            {
                writer.WriteLine(new BaseResponse() { 
                    Message = string.Format("Platform {0} was not found in the DB.", binary.Platform)
                }.ToJSON());
                return false;
            }
            return true;
        }

        public static bool DownloadFile(BinaryInfo binary, StreamWriter writer)
        {
            int result = binary.Download();
            if (result == -1)
            {
                writer.WriteLine(new BaseResponse() { Message = "SHA256 hash mismatch for one of the files downloaded." }.ToJSON());
                return false;
            }
            else if (result != 1)
            {
                writer.WriteLine(new BaseResponse() { Message = "An unknown error occurred when downloading a file." }.ToJSON());
                return false;
            }
            return true;
        }

        public static bool AddVersionToBOINC(AddBinaryRequest request, StreamWriter writer, long appID)
        {
            BinaryInfo binary = request.Binary;
            string xml = GenerateXML(binary, request.Codename);

            if(!BoincDatabaseManager.AddAppVersion(appID, binary.VersionNumber, 1, xml.ToString(), binary.HardwareAccelerator)) {
                writer.WriteLine(new BaseResponse()
                {
                    Message = string.Format("Failed to add version info to the DB.", binary.VersionNumber, binary.Platform, binary.HardwareAccelerator)
                }.ToJSON());
                return false;
            }
            return true;
        }

        public static string GenerateXML(BinaryInfo binary, string codename)
        {
            StringBuilder xml = new StringBuilder();
            AppVersion version = new AppVersion();
            version.Name = codename;
            version.VersionNumber = binary.VersionNumber;
            version.Reference = new FileReference()
            {
                FileName = new Uri(binary.BinaryURL).Segments.Last()
            };

            xml.Append(binary.ToBoincFormat().Serialize());
            xml.Append("\n");
            xml.Append(version.Serialize());
            return xml.ToString();
        }
    }
}
