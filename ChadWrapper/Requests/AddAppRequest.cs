using ChadWrapper.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChadWrapper.Requests
{
    class AddAppRequest
    {
        public string Codename;
        public string FriendlyName;
        public List<BinaryInfo> Binaries;

        public bool IsValid()
        {
            if (Codename == null) return false;
            if (FriendlyName == null) return false;
            if (Binaries == null || Binaries.Count == 0) return false;

            foreach(var binary in Binaries)
            {
                if (!binary.IsValid())
                    return false;
            }

            return true;
        }

        public static AddAppRequest FromJSON(string jsonStr)
        {
            try
            {
                return JsonConvert.DeserializeObject<AddAppRequest>(jsonStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
