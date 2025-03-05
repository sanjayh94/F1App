using Newtonsoft.Json;

namespace F1Api.Models
{
    public class DriverStanding
    {
        [JsonProperty("driverStandingsId")]
        public int Id { get; set; }
        
        public int RaceId { get; set; }

        public int DriverId { get; set; }

        public float Points { get; set; }

        public int Position { get; set; }

        public int? PositionText { get; set; }

        public int Wins { get; set; }
    }
}