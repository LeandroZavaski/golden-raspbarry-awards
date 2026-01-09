namespace GoldenRaspberryAwards.Domain.Interfaces
{
    public interface ICsvImportService
    {
        Task ImportAsync(string filePath);
    }
}
