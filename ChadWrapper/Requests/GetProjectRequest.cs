using ChadWrapper.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChadWrapper.Requests
{
    class GetProjectRequest
    {
        public string Codename;

        public bool IsValid()
        {
            if (Codename == null) return false;

            return true;
        }

        public static GetProjectRequest FromJSON(string jsonStr)
        {
            try
            {
                return JsonConvert.DeserializeObject<GetProjectRequest>(jsonStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
