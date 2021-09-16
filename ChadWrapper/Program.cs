using ChadWrapper.Boinc;
using ChadWrapper.Data;
using Newtonsoft.Json;
using System;
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

            if (!DatabaseManager.IsSetup())
            {
                Console.WriteLine("Could not connect to the DB.");
                return;
            }

            APIServer api = new APIServer(80);

            api.SetBaseURL("/chadwrapper");
            api.AddAction("/addapp", Handlers.AddApp.ProcessContext);

            Global.ApiServer = api;
            api.Listen();
        }
    }
}
