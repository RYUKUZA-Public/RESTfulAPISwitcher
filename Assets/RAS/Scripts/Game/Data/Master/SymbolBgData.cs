using Newtonsoft.Json;

namespace Master
{
    public class SymbolBgData : ModelBase
    {
        [JsonProperty("key")]
        public string key { get; set; }
    }
}