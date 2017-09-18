using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ApiaryCompetition.Api
{
    public sealed class CamelCaseJsonConvert
    {
        readonly JsonSerializerSettings settings;

        public CamelCaseJsonConvert()
        {
            settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public string Serialize<T>(T @object) => JsonConvert.SerializeObject(@object, settings);

        public T Deserialize<T>(string jsonObject) => JsonConvert.DeserializeObject<T>(jsonObject, settings);
    }
}
