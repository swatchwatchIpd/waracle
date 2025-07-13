using WaracleTestAPI.DTOs;

namespace WaracleTestAPI.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(DateTime checkInDate, DateTime checkOutDate, int guestCount, int? hotelId = null);
        Task<RoomDto?> GetRoomByIdAsync(int id);
        Task<IEnumerable<RoomDto>> GetRoomsByHotelIdAsync(int hotelId);
    }
} 