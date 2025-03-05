using System;

namespace F1Api.Models
{
    public class CircuitSummary
    {
        public int CircuitId { get; set; }
        public string CircuitReference { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Country { get; set; }
        public string FastestLapTime { get; set; }
        public double? FastestLapTimeMilliseconds { get; set; }
        public string FastestLapDriver { get; set; }
        public int FastestLapRaceYear { get; set; }
        public int TotalRacesCompleted { get; set; }
    }
}