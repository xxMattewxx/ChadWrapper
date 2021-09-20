using ChadWrapper.Boinc;
using ChadWrapper.Data;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace ChadWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.ToLower() == "--debug") Global.Debug = true;
            }

            Global.Load();

            for (int i = 0; !DatabaseManager.IsSetup(); i++)
            {
                if(i == 0)
                    Console.WriteLine("Waiting for the database to be setup.");

                if (i == 60)
                {
                    Console.WriteLine("Exceeded timeout waiting for the database to be setup.");
                    return;
                }

                Thread.Sleep(1000);
            }

            APIServer api = new APIServer(80);

            api.SetBaseURL("/chadwrapper");
            api.AddAction("/addapp", Handlers.AddApp.ProcessContext);
            api.AddAction("/getproject", Handlers.GetProjectInfo.ProcessContext);

            Global.ApiServer = api;
            api.Listen();
        }
    }
}
