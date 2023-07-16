using Newtonsoft.Json;

namespace Master
{
    public class SymbolCharacterData : ModelBase
    {
        [JsonProperty("key")]
        public string key { get; set; }
    }
}