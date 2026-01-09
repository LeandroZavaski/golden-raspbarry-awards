namespace GoldenRaspberryAwards.Domain.Dtos
{
    public class ProducerIntervalResponseDto
    {
        public List<ProducerIntervalDto> Min { get; set; } = new();
        public List<ProducerIntervalDto> Max { get; set; } = new();
    }
}
