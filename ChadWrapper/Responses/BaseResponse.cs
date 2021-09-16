using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChadWrapper.Responses
{
    class BaseResponse
    {
        public bool Success;
        public string Message;

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
