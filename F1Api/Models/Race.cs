using Newtonsoft.Json;

namespace F1Api.Models
{
    public class Race
    {
        [JsonProperty("raceId")]
        public int Id { get; set; }

        public int Year { get; set; }

        public int Round { get; set; }

        public int CircuitId { get; set; }

        public string Name { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly? Time { get; set; }

        public Uri? Url { get; set; }

        [JsonProperty("fp1_date")]
        public DateOnly? FreePractice1Date { get; set; }

        [JsonProperty("fp1_time")]
        public TimeOnly? FreePractice1Time { get; set; }

        [JsonProperty("fp2_date")]
        public DateOnly? FreePractice2Date { get; set; }

        [JsonProperty("fp2_time")]
        public TimeOnly? FreePractice2Time { get; set; }

        [JsonProperty("fp3_date")]
        public DateOnly? FreePractice3Date { get; set; }

        [JsonProperty("fp3_time")]
        public TimeOnly? FreePractice3Time { get; set; }

        [JsonProperty("quali_date")]
        public DateOnly? QualificationDate { get; set; }

        [JsonProperty("quali_time")]
        public TimeOnly? QualificationTime { get; set; }

        [JsonProperty("sprint_date")]
        public DateOnly? SprintDate { get; set; }

        [JsonProperty("sprint_time")]
        public TimeOnly? SprintTime { get; set; }
    }
}