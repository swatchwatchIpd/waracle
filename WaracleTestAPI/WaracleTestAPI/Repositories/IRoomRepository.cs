using WaracleTestAPI.Models;

namespace WaracleTestAPI.Repositories
{
    public interface IRoomRepository
    {
        Task<Room?> GetRoomByIdAsync(int id);
        Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(int hotelId);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkInDate, DateTime checkOutDate, int guestCount, int? hotelId = null);
        Task<Room> CreateRoomAsync(Room room);
        Task<Room> UpdateRoomAsync(Room room);
        Task<bool> DeleteRoomAsync(int id);
        Task<bool> RoomExistsAsync(int id);
    }
} 