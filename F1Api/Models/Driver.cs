using Newtonsoft.Json;

namespace F1Api.Models
{
    public class Driver
    {
        [JsonProperty("driverId")]
        public int Id { get; set; }

        [JsonProperty("driverRef")]
        public string DriverReference { get; set; }

        public int? Number { get; set; }

        public string? Code { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        [JsonProperty("dob")]
        public DateOnly? DateOfBirth { get; set; }

        public string Nationality { get; set; }

        public Uri? Url { get; set; }
    }
}