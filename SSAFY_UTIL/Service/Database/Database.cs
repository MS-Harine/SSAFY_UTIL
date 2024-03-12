using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSAFY_UTIL.Service.Database
{
    internal interface Database
    {
        Task<JObject> Create(string DataIdentifier, JObject data);
        Task<JArray> Read(string DataIdentifier, uint limit = 0);
        Task<JObject> Update(string DataIdentifier, JObject data);
        Task<JObject> Delete(string DataIdentifier);
    }
}
