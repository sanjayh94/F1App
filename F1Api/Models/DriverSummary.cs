using System;

namespace F1Api.Models
{
    public class DriverSummary
    {
        public int DriverId { get; set; }
        public string DriverReference { get; set; }
        public string FullName { get; set; }
        public string Nationality { get; set; }
        public int PodiumCount { get; set; }
        public int TotalRacesEntered { get; set; }
    }
}