namespace WaracleTestAPI.Services
{
    public interface IDataService
    {
        Task ResetDataAsync();
        Task SeedDataAsync();
        Task<object> GetDatabaseStatsAsync();
    }
} 