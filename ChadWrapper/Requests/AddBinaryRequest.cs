using ChadWrapper.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChadWrapper.Requests
{
    class AddBinaryRequest
    {
        public string Codename;
        public BinaryInfo Binary;

        public bool IsValid()
        {
            if (Codename == null || !Utils.IsSafeString(Codename)) return false;
            if (Binary == null || !Binary.IsValid()) return false;

            return true;
        }

        public static AddBinaryRequest FromJSON(string jsonStr)
        {
            try
            {
                return JsonConvert.DeserializeObject<AddBinaryRequest>(jsonStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
