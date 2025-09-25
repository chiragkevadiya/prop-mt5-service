using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Extension
{
    public static class JsonExtensions
    {
        public static T DeserializeCamelCase<T>(string json)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    NamingStrategy = new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy()
                }
            };

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
