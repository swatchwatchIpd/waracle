using WaracleTestAPI.Repositories;

namespace WaracleTestAPI.Services
{
    public class DataService : IDataService
    {
        private readonly IDataRepository _dataRepository;

        public DataService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ResetDataAsync()
        {
            await _dataRepository.ResetDataAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task SeedDataAsync()
        {
            // Just seed the data - user should call reset first if needed
            await _dataRepository.SeedDataAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetDatabaseStatsAsync()
        {
            var stats = await _dataRepository.GetDatabaseStatsAsync();
            return new
            {
                hotels = stats.hotels,
                rooms = stats.rooms,
                roomTypes = stats.roomTypes,
                bookings = stats.bookings
            };
        }
    }
} 