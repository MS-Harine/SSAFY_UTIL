using Newtonsoft.Json.Linq;
using SSAFY_UTIL.Service.Networking;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SSAFY_UTIL.Service.Database
{
    class Firebase : NetworkingBase, Database
    {
        private static readonly string DATABASE_URI = "https://ssafy-11-default-rtdb.asia-southeast1.firebasedatabase.app/ssafy_util/";
        private static readonly Lazy<Firebase> lazy = new Lazy<Firebase>(() => new Firebase());
        private static Firebase Instance { get { return lazy.Value; } }

        private Firebase() : base(new Uri(DATABASE_URI), new List<KeyValuePair<string, string>> {
            new("User-Agent", "WINUI3 (HTTPCLIENT)"),
            new("Accept", "application/json")
        })
        { }

        private Dictionary<string, object> cache = new();
        private Dictionary<string, DateTime> cacheTime = new();

        public static Firebase GetInstance()
        {
            return Instance;
        }

        private object CheckCache(string Identifier)
        {
            if (cache.ContainsKey(Identifier))
            {
                if ((DateTime.Now - cacheTime[Identifier]).TotalSeconds < 60)
                    return cache[Identifier];
            }
            return null;
        }

        private void SetCache(string Identifier, object data)
        {
            cache[Identifier] = data;
            cacheTime[Identifier] = DateTime.Now;
        }

        public async Task<JObject> Create(string DataIdentifier, JObject data)
        {
            JObject CacheHit = (JObject)CheckCache(DataIdentifier);
            if (CacheHit != null)
            {
                return CacheHit;
            }

            var (code, resultString) = await Request(HttpMethod.Put, DataIdentifier, null, data.ToString(), null);
            if (code != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Database Create Error with code " + code.ToString());
            }

            JObject result = JObject.Parse(resultString);
            SetCache(DataIdentifier, result);

            return result;
        }

        public async Task<JArray> Read(string DataIdentifier, uint limit = 0)
        {
            JArray CacheHit = (JArray)CheckCache(DataIdentifier);
            if (CacheHit != null)
            {
                return CacheHit;
            }

            var (code, resultString) = await Request(HttpMethod.Get, DataIdentifier, null, String.Empty, null);
            if (code != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Database Read Error with code " + code.ToString());
            }

            JArray result = default;
            if (resultString == "null")
                return result;

            result = JArray.Parse(resultString);
            SetCache(DataIdentifier, result);

            return result;
        }

        public async Task<JObject> Update(string DataIdentifier, JObject data)
        {
            var (code, resultString) = await Request(HttpMethod.Patch, DataIdentifier, null, data.ToString(), null);
            if (code != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Database Create Error with code " + code.ToString());
            }

            JObject result = JObject.Parse(resultString);
            SetCache(DataIdentifier, result);

            return result;
        }

        public async Task<JObject> Delete(string DataIdentifier)
        {
            var (code, resultString) = await Request(HttpMethod.Delete, DataIdentifier, null, String.Empty, null);
            if (code != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Database Create Error with code " + code.ToString());
            }

            JObject result = JObject.Parse(resultString);
            SetCache(DataIdentifier, null);

            return result;
        }
    }
}
