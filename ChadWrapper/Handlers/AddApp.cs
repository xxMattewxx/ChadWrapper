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

            /* TODO IMPLEMENT AUTH UserInfo user = UserInfo.GetFromAPIKey(APIKey);
            if (user == null || !user.CanAddTasks())
            {
                writer.WriteLine(new BaseResponse() { Message = "API Key not authorized!" }.ToJSON());
                return;
            }*/

            if (!CheckPlatformsExist(request.Binaries, writer))
                return;

            /* Code Signing Key will be in the database and should be decrypted with a key stored outside of the container */
            ConfigEntry codeSigningKey = ConfigEntry.Get("CODE_SIGNING_PRIVATE");
            if (codeSigningKey == null || codeSigningKey.Value == null)
            {
                writer.WriteLine(new BaseResponse() { Message = "Unable to retrieve code signing key from DB." }.ToJSON());
                return;
            }

            /* DownloadFiles should check the SHA256 hash of each binary provided to ensure it matches, as a checksum */
            /* This function should also populate the binaries missing data such as byte length */
            if (!DownloadFiles(request.Binaries, writer))
                return;

            /* TODO Port BOINC code to C#
            if (!Utils.SignFiles(request.Binaries, codeSigningKey))
            {
                writer.WriteLine(new BaseResponse() { Message = "Failure signing downloaded binaries." }.ToJSON());
                return;
            }*/

            long appID = AddAppToBOINC(request, writer);
            if (appID == -1)
                return;

            if (!AddVersionsToBOINC(request, writer, appID))
                return;

            writer.WriteLine(new AddAppResponse() {
                Success = true,
                Message = "App added successfully.",
                Binaries = request.Binaries
            }.ToJSON());
        }

        public static bool CheckPlatformsExist(List<BinaryInfo> binaries, StreamWriter writer)
        {
            foreach(var binary in binaries)
            {
                if (BoincDatabaseManager.GetPlatformID(binary.Platform) == -1)
                {
                    writer.WriteLine(new BaseResponse() { 
                        Message = string.Format("Platform {0} was not found in the DB.", binary.Platform)
                    }.ToJSON());
                    return false;
                }
            }
            return true;
        }

        public static bool DownloadFiles(List<BinaryInfo> binaries, StreamWriter writer)
        {
            foreach (var binary in binaries)
            {
                int result = binary.Download();
                if (result == -1)
                {
                    writer.WriteLine(new BaseResponse() { Message = "SHA256 hash mismatch for one of the files downloaded." }.ToJSON());
                    return false;
                }
                else if(result != 1)
                {
                    writer.WriteLine(new BaseResponse() { Message = "An unknown error occurred when downloading a file." }.ToJSON());
                    return false;
                }
            }
            return true;
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
                Console.WriteLine("App {0} already exists in DB.", request.Codename);
                appID = BoincDatabaseManager.GetAppID(request.Codename);
                if(appID == -1)
                {
                    writer.WriteLine(new BaseResponse() { Message = "Could not retrieve existing app info from the BOINC DB." }.ToJSON());
                    return -1;
                }
            }

            return appID;
        }

        public static bool AddVersionsToBOINC(AddAppRequest request, StreamWriter writer, long appID)
        {
            foreach (BinaryInfo binary in request.Binaries)
            {
                string xml = GenerateXML(binary, request.Codename);
                Console.WriteLine("({0})", xml.ToString());
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
