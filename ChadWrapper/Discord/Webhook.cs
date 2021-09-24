using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChadWrapper.Discord
{
    class Webhook
    {
        public static bool SendMessage(string webhookURL, string message)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(Global.AlertsWebhookURL);

                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("username", "Chad Wrapper");
                dict.Add("content", message);

                var postData = JsonConvert.SerializeObject(dict);
                var data = Encoding.UTF8.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                using var streamReader = new StreamReader(response.GetResponseStream());
                var str = streamReader.ReadToEnd();
                Console.WriteLine(str);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
