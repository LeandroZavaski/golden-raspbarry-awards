namespace GoldenRaspberryAwards.Domain.Dtos
{
    public class ProducerIntervalDto
    {
        public string Producer { get; set; } = string.Empty;
        public int Interval { get; set; }
        public int PreviousWin { get; set; }
        public int FollowingWin { get; set; }
    }
}
