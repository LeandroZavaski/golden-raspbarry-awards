namespace GoldenRaspberryAwards.Domain.Pipelines
{
    public interface IProducerIntervalPipelineStep<TInput, TOutput>
    {
        Task<TOutput> ExecuteAsync(TInput input);
    }
}
