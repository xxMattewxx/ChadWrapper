using ChadWrapper.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChadWrapper.Responses
{
    class AddBinaryResponse : BaseResponse
    {
        public BinaryInfo Binary { get; set; }
    }
}
