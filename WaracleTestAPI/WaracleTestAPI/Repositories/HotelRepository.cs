using Microsoft.EntityFrameworkCore;
using WaracleTestAPI.Data;
using WaracleTestAPI.Models;

namespace WaracleTestAPI.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelBookingDbContext _context;

        public HotelRepository(HotelBookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelsAsync()
        {
            return await _context.Hotels.ToListAsync();
        }

        public async Task<Hotel?> GetHotelByIdAsync(int id)
        {
            return await _context.Hotels
                .Include(h => h.Rooms)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(h => h.HotelId == id);
        }

        public async Task<IEnumerable<Hotel>> SearchHotelsByNameAsync(string name)
        {
            return await _context.Hotels
                .Where(h => h.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<Hotel> CreateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }

        public async Task<Hotel> UpdateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }

        public async Task<bool> DeleteHotelAsync(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                return false;

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HotelExistsAsync(int id)
        {
            return await _context.Hotels.AnyAsync(h => h.HotelId == id);
        }
    }
} 