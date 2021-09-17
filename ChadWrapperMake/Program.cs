using System;
using System.Reflection;
using System.Threading;
using ChadWrapperMake.Data;

namespace ChadWrapperMake
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

            for(int i = 0; !DatabaseManager.IsDBRunning(); i++)
            {
                if(i == 0)
                    Console.WriteLine("Waiting for the Postgres server to start.");

                if (i == 60)
                {
                    Console.WriteLine("Exceeded timeout waiting for the Postgres server to start.");
                    return;
                }

                Thread.Sleep(1000);
            }

            if(DatabaseManager.IsSetup())
            {
                Console.WriteLine("Chad Wrapper DB already exists. Assuming everything is fine.");
                return;
            }

            if(!DatabaseManager.CreateDB())
            {
                Console.WriteLine("Could not create Chad Wrapper DB.");
                return;
            }

            if(!DatabaseManager.CreateTables())
            {
                Console.WriteLine("Could not create the required tables!");
                return;
            }

            ConfigEntry isSetup = new ConfigEntry();
            isSetup.Key = "IS_SETUP";

            if (!isSetup.Save())
            {
                Console.WriteLine("Could not save the init flag!");
                return;
            }
        }
    }
}
