using ChadWrapper.Boinc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChadWrapper.Data.Types
{
    class BinaryInfo
    {
        public string Platform { get; set; }
        public int VersionNumber { get; set; }
        public string BinaryURL { get; set; }
        public string FileHash { get; set; }
        public Int64 FileSize { get; set; }
        public string FileSignature { get; set; }
        public string HardwareAccelerator { get; set; }

        public bool IsValid()
        {
            if (BinaryURL == null || !Uri.TryCreate(BinaryURL, UriKind.RelativeOrAbsolute, out _)) return false;
            if (Platform == null || Platform.Length < 1) return false;
            if (VersionNumber < 1) return false;
            if (FileHash == null) return false;
            if (FileSignature == null) return false;
            if (HardwareAccelerator == null) return false;
            return true;
        }

        /* 
             Returns 0 in case of an exception (couldn't connect to the server, don't have permission to write, etc)
             1 in case of a success
             -1 if the file hash provided doesn't match the file hash received from the URL
        */
        public int Download()
        {
            try
            {
                var url = new Uri(BinaryURL);
                var fileName = Global.BinariesFolder + url.Segments.Last();

                using WebClient webClient = new WebClient();
                webClient.DownloadFile(BinaryURL, fileName);

                Console.WriteLine("Downloaded file.");

                string hash = Utils.SHA256File(fileName);
                if (hash != FileHash.ToLower())
                    return -1;

                FileInfo fileInfo = new FileInfo(fileName);
                FileSize = fileInfo.Length;

                Console.WriteLine("Downloaded file with hash {0}", hash);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public string GetMD5()
        {
            try
            {
                var url = new Uri(BinaryURL);
                var fileName = Global.BinariesFolder + url.Segments.Last();
                return Utils.GenerateHexString(Utils.CalculateMD5(fileName));
            }
            catch
            {
                return null;
            }
        }

        public BoincFileInfo ToBoincFormat()
        {
            BoincFileInfo ret = new BoincFileInfo();
            var url = new Uri(BinaryURL);
            ret.ByteCount = FileSize;
            ret.IsExecutable = "";
            ret.FileSignature = FileSignature;
            ret.URL = BinaryURL;
            ret.Name = url.Segments.Last();
            return ret;
        }

        public static BinaryInfo FromBoincXML(string xml)
        {
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml("<root>" + xml + "</root>");
            }
            catch(XmlException e)
            {
                Console.WriteLine(e);
                return null;
            }

            BinaryInfo ret = new BinaryInfo();
            var fileInfo = doc.DocumentElement.SelectSingleNode("file_info");
            ret.BinaryURL = fileInfo.SelectSingleNode("url").InnerText;

            var appVersion = doc.DocumentElement.SelectSingleNode("app_version");
            ret.VersionNumber = Convert.ToInt32(appVersion.SelectSingleNode("version_num").InnerText);
            return ret;
        }
    }
}
