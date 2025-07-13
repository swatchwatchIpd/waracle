using WaracleTestAPI.DTOs;
using WaracleTestAPI.Repositories;
using WaracleTestAPI.Utilities;

namespace WaracleTestAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;

        public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="createBookingDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto)
        {
            // Basic date validation
            if (createBookingDto.CheckInDate >= createBookingDto.CheckOutDate)
                throw new ArgumentException("Check-in date must be before check-out date");

            if (createBookingDto.CheckInDate.Date <= DateTime.Today)
                throw new ArgumentException("Check-in date cannot be in the past");

            // Validate room exists and get room details
            var room = await _roomRepository.GetRoomByIdAsync(createBookingDto.RoomId);
            if (room == null)
                throw new ArgumentException($"Room with ID {createBookingDto.RoomId} not found");

            //  Business Rule: Check room capacity
            if (createBookingDto.GuestCount > room.RoomType!.Capacity)
                throw new ArgumentException($"Guest count ({createBookingDto.GuestCount}) exceeds room capacity ({room.RoomType.Capacity})");

            if (createBookingDto.GuestCount <= 0)
                throw new ArgumentException("Guest count must be at least 1");

            // Business Rule: Prevent double booking (overlapping dates)
            var hasOverlappingBooking = await _bookingRepository.HasOverlappingBookingAsync(
                createBookingDto.RoomId,
                createBookingDto.CheckInDate,
                createBookingDto.CheckOutDate);

            if (hasOverlappingBooking)
                throw new InvalidOperationException("Room is already booked during the selected dates");

            //  Business Rule: Generate unique booking number
            var bookingNumber = await GenerateUniqueBookingNumberAsync();

            //  Business Rule: Validate booking number uniqueness (extra safety check)
            var existingBooking = await _bookingRepository.BookingExistsAsync(bookingNumber);
            if (existingBooking)
                throw new InvalidOperationException($"Booking number {bookingNumber} already exists");

            // 7. Create booking with all validations passed
            var booking = createBookingDto.ToModel();
            booking.BookingNumber = bookingNumber;
            booking.CreatedAt = DateTime.UtcNow;

            var createdBooking = await _bookingRepository.CreateBookingAsync(booking);
            
            // Final verification - retrieve complete booking with relationships
            var completedBooking = await _bookingRepository.GetBookingByNumberAsync(createdBooking.BookingNumber);
            if (completedBooking == null)
                throw new InvalidOperationException("Failed to retrieve the created booking");
            
            return completedBooking.ToDto();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookingNumber"></param>
        /// <returns></returns>
        public async Task<BookingDto?> GetBookingByNumberAsync(string bookingNumber)
        {
            if (string.IsNullOrWhiteSpace(bookingNumber))
                return null;

            var booking = await _bookingRepository.GetBookingByNumberAsync(bookingNumber);
            return booking?.ToDto();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BookingDto>> GetBookingsByRoomIdAsync(int roomId)
        {
            var bookings = await _bookingRepository.GetBookingsByRoomIdAsync(roomId);
            return bookings.ToBookingDtos();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookingNumber"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBookingAsync(string bookingNumber)
        {
            if (string.IsNullOrWhiteSpace(bookingNumber))
                return false;

            return await _bookingRepository.DeleteBookingByNumberAsync(bookingNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<string> GenerateUniqueBookingNumberAsync()
        {
            string bookingNumber;
            int attempts = 0;
            const int maxAttempts = 10;

            do
            {
                attempts++;
                if (attempts > maxAttempts)
                    throw new InvalidOperationException("Unable to generate unique booking number after multiple attempts");

                // Generate booking number: BK + current date + random 4-digit number
                var datePart = DateTime.Now.ToString("yyyyMMdd");
                var randomPart = new Random().Next(1000, 9999);
                bookingNumber = $"BK{datePart}{randomPart}";

                // Check if this booking number already exists
                var exists = await _bookingRepository.BookingExistsAsync(bookingNumber);
                if (!exists)
                    break;
            }
            while (true);

            return bookingNumber;
        }
    }
} 