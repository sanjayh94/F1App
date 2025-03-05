using Newtonsoft.Json;

namespace F1Api.Models
{
    public class Circuit
    {
        [JsonProperty("circuitId")]
        public int Id { get; set; }

        [JsonProperty("circuitRef")]
        public string CircuitReference { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string Country { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }

        public int? Alt { get; set; }

        public Uri Url { get; set; }
    }
}