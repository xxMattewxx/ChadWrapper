using ChadWrapper.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChadWrapper.Responses.Tasks
{
    class AddSequentialResponse : BaseResponse
    {
        public Int64 TasksCount { get; set; }
    }
}
