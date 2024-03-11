using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSAFY_UTIL.Service.Database
{
    interface Database
    {
        Task<T> Create<T>(string DataIdentifier, T data);
        Task<List<T>> Read<T>(string DataIdentifier, uint limit = 0);
        Task<bool> Update(string DataIdentifier, Dictionary<string, object> data);
        Task<bool> Delete(string DataIdentifier);
    }
}
