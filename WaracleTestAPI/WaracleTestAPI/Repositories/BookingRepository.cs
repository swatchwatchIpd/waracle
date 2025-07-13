using Microsoft.EntityFrameworkCore;
using WaracleTestAPI.Data;
using WaracleTestAPI.Models;

namespace WaracleTestAPI.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly HotelBookingDbContext _context;

        public BookingRepository(HotelBookingDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<Booking?> GetBookingByNumberAsync(string bookingNumber)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRoomIdAsync(int roomId)
        {
            return await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.Hotel)
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .Where(b => b.RoomId == roomId)
                .OrderBy(b => b.CheckInDate)
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingBookingAsync(int roomId, DateTime checkInDate, DateTime checkOutDate, int? excludeBookingId = null)
        {
            return await _context.Bookings
                .Where(b => b.RoomId == roomId)
                .Where(b => excludeBookingId == null || b.BookingId != excludeBookingId)
                .AnyAsync(b => !(checkOutDate <= b.CheckInDate || checkInDate >= b.CheckOutDate));
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            
            // Retrieve the created booking with all relationships
            var createdBooking = await GetBookingByNumberAsync(booking.BookingNumber);
            return createdBooking ?? booking;
        }

        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookingByNumberAsync(string bookingNumber)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);
            
            if (booking == null)
                return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> BookingExistsAsync(string bookingNumber)
        {
            return await _context.Bookings.AnyAsync(b => b.BookingNumber == bookingNumber);
        }

        public async Task<string> GenerateUniqueBookingNumberAsync()
        {
            string bookingNumber;
            bool isUnique;

            do
            {
                // Generate booking number: BK + current date + random 4-digit number
                var datePart = DateTime.Now.ToString("yyyyMMdd");
                var randomPart = new Random().Next(1000, 9999);
                bookingNumber = $"BK{datePart}{randomPart}";

                // Check if this booking number already exists
                isUnique = !await _context.Bookings.AnyAsync(b => b.BookingNumber == bookingNumber);
            }
            while (!isUnique);

            return bookingNumber;
        }
    }
} 