using GoldenRaspberryAwards.Domain.Dtos;

namespace GoldenRaspberryAwards.Domain.Interfaces
{
    public interface IProducerService
    {
        Task<ProducerIntervalResponseDto> GetProducerIntervalsAsync();
    }
}
