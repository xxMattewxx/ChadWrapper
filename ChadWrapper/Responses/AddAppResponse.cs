using ChadWrapper.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChadWrapper.Responses
{
    class AddAppResponse : BaseResponse
    {
        public List<BinaryInfo> Binaries;
    }
}
