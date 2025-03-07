using System;

namespace F1Api.Models
{
    public class DriverChampionshipSummary
    {
        public int DriverId { get; set; }
        public string DriverReference { get; set; }
        public string FullName { get; set; }
        public float TotalPoints { get; set; }
        public int Position { get; set; }
        public int Wins { get; set; }
    }
}