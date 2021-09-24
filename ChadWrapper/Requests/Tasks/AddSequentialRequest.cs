using ChadWrapper.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChadWrapper.Requests.Tasks
{
    class AddSequentialRequest
    {
        public string Codename { get; set; }
        public Int64 Start { get; set; }
        public Int64 End { get; set; }
        public Int64 TaskSize { get; set; }

        public bool IsValid()
        {
            if (Codename == null || !Utils.IsSafeString(Codename)) return false;
            if (End < Start) return false;
            if (TaskSize <= 0) return false;

            return true;
        }

        public static AddSequentialRequest FromJSON(string jsonStr)
        {
            try
            {
                return JsonConvert.DeserializeObject<AddSequentialRequest>(jsonStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
