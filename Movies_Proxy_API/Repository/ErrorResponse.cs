using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Movies_Proxy_API.Repository
{
    internal class ErrorResponse
    {
        public string ErrorMessage { get; internal set; }
        public int StatusCode { get; internal set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}