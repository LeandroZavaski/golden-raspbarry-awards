namespace GoldenRaspberryAwards.Infrastructure.Pipelines
{
    /// <summary>
    /// Interface base para cada etapa do pipeline de processamento de intervalos de produtores.
    /// </summary>
    /// <typeparam name="TInput">Tipo de entrada da etapa</typeparam>
    /// <typeparam name="TOutput">Tipo de saída da etapa</typeparam>
    public interface IProducerIntervalPipelineStep<TInput, TOutput>
    {
        /// <summary>
        /// Executa a transformação de dados da etapa.
        /// </summary>
        Task<TOutput> ExecuteAsync(TInput input);
    }
}
