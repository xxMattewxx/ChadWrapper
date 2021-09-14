using System;

namespace ChadWrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.Load();

            APIServer api = new APIServer(80);

            api.SetBaseURL("/chadwrapper");

            Global.ApiServer = api;
            api.Listen();
        }
    }
}
