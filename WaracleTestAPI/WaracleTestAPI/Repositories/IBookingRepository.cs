using WaracleTestAPI.Models;

namespace WaracleTestAPI.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<Booking?> GetBookingByNumberAsync(string bookingNumber);
        Task<IEnumerable<Booking>> GetBookingsByRoomIdAsync(int roomId);
        Task<bool> HasOverlappingBookingAsync(int roomId, DateTime checkInDate, DateTime checkOutDate, int? excludeBookingId = null);
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int id);
        Task<bool> DeleteBookingByNumberAsync(string bookingNumber);
        Task<bool> BookingExistsAsync(string bookingNumber);
        Task<string> GenerateUniqueBookingNumberAsync();
    }
} 