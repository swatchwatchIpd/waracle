using WaracleTestAPI.Models;

namespace WaracleTestAPI.Repositories
{
    public interface IHotelRepository
    {
        Task<IEnumerable<Hotel>> GetAllHotelsAsync();
        Task<Hotel?> GetHotelByIdAsync(int id);
        Task<IEnumerable<Hotel>> SearchHotelsByNameAsync(string name);
        Task<Hotel> CreateHotelAsync(Hotel hotel);
        Task<Hotel> UpdateHotelAsync(Hotel hotel);
        Task<bool> DeleteHotelAsync(int id);
        Task<bool> HotelExistsAsync(int id);
    }
} 