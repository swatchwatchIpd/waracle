using WaracleTestAPI.DTOs;

namespace WaracleTestAPI.Services
{
    public interface IBookingService
    {
        Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto);
        Task<BookingDto?> GetBookingByNumberAsync(string bookingNumber);
        Task<IEnumerable<BookingDto>> GetBookingsByRoomIdAsync(int roomId);
        Task<bool> DeleteBookingAsync(string bookingNumber);
    }
} 