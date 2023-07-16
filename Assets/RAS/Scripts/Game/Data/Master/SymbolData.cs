using Newtonsoft.Json;

namespace Master
{
    public class SymbolData : ModelBase
    {
        [JsonProperty("bgId")]
        public uint bgId { get; set;}

        [JsonProperty("frameId")]
        public uint frameId { get; set;}

        [JsonProperty("characterId")]
        public uint characterId { get; set;}
    }
}