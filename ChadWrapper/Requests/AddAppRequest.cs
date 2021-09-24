﻿using ChadWrapper.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ChadWrapper.Requests
{
    class AddAppRequest
    {
        public string Codename { get; set; }
        public string FriendlyName { get; set; }

        public bool IsValid()
        {
            if (Codename == null || !Utils.IsSafeString(Codename)) return false;
            if (FriendlyName == null || Codename.Length == 0) return false;

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
