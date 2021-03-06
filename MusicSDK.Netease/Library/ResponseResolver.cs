using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MusicSDK.Netease.Library
{
    public class ResponseResolver
    {
        private readonly Task<string> _task;
        static readonly JsonSerializerSettings defaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    OverrideSpecifiedNames = false
                }
            },
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
        };
        public ResponseResolver(Task<string> task)
        {
            _task = task;
        }
        public async Task<T> Into<T>(JsonSerializerSettings? settings = null)
        {
            var str = await _task;
            return JsonConvert.DeserializeObject<T>(str, settings ?? defaultSettings)!;
        }


        public async Task<T> Into<T>(T typeObject, JsonSerializerSettings? settings = null)
        {
            var str = await _task;
            return JsonConvert.DeserializeAnonymousType(str, typeObject, settings ?? defaultSettings);
        }

        public async Task<string> Into()
        {
            return await _task;
        }
    }
}