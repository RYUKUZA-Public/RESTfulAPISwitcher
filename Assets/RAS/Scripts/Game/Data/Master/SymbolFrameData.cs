using Newtonsoft.Json;

namespace Master
{
    public class SymbolFrameData : ModelBase
    {
        [JsonProperty("key")]
        public string key { get; set; }
    }
}