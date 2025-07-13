using Microsoft.EntityFrameworkCore;
using WaracleTestAPI.Data;
using WaracleTestAPI.Models;

namespace WaracleTestAPI.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelBookingDbContext _context;

        public RoomRepository(HotelBookingDbContext context)
        {
            _context = context;
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.RoomId == id);
        }

        public async Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(int hotelId)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Where(r => r.HotelId == hotelId)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkInDate, DateTime checkOutDate, int guestCount, int? hotelId = null)
        {
            return await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Where(r => r.RoomType.Capacity >= guestCount) // Room can accommodate the number of guests
                .Where(r => hotelId == null || r.HotelId == hotelId) // Filter by hotel if specified
                .Where(r => !r.Bookings.Any(b => // No overlapping bookings
                    !(checkOutDate <= b.CheckInDate || checkInDate >= b.CheckOutDate)))
                .OrderBy(r => r.Hotel.Name)
                .ThenBy(r => r.RoomNumber)
                .ToListAsync();
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room> UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RoomExistsAsync(int id)
        {
            return await _context.Rooms.AnyAsync(r => r.RoomId == id);
        }
    }
} 