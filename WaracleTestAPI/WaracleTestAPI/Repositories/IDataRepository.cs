namespace WaracleTestAPI.Repositories
{
    public interface IDataRepository
    {
        Task ResetDataAsync();
        Task SeedDataAsync();
        Task<(int hotels, int rooms, int roomTypes, int bookings)> GetDatabaseStatsAsync();
        Task<bool> HasDataAsync();
    }
} 