using System;
using System.Reflection;
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

            ConfigEntry codeSigningKey = new ConfigEntry();
            codeSigningKey.Key = "CODE_SIGNING_PRIVATE";
            codeSigningKey.Value = Global.CodeSigningPrivateKey;
            
            if(!codeSigningKey.Save()) {
                Console.WriteLine("Could not save the private key!");
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
