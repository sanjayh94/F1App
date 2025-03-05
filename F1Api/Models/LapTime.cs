namespace F1Api.Models
{
    public class LapTime
    {
        public int RaceId { get; set; }

        public int DriverId { get; set; }

        public int Lap { get; set; }

        public int Position { get; set; }

        public string? Time { get; set;}

        public double? Milliseconds { get; set; }
    }
}