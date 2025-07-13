using WaracleTestAPI.DTOs;

namespace WaracleTestAPI.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelSearchDto>> GetAllHotelsAsync();
        Task<HotelDto?> GetHotelByIdAsync(int id);
        Task<IEnumerable<HotelSearchDto>> SearchHotelsByNameAsync(string name);
    }
} 