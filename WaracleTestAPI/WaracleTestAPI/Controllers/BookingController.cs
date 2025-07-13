using Microsoft.AspNetCore.Mvc;
using WaracleTestAPI.DTOs;
using WaracleTestAPI.Services;

namespace WaracleTestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Create a new booking
        /// Note: CheckInDate and CheckOutDate should be in DD/MM/YYYY format (e.g., 25/12/2024)
        /// </summary>
        /// <param name="createBookingDto">Booking details with dates in DD/MM/YYYY format</param>
        /// <returns>Created booking</returns>
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto createBookingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var bookingDto = await _bookingService.CreateBookingAsync(createBookingDto);
                return CreatedAtAction(nameof(GetBooking), new { bookingNumber = bookingDto.BookingNumber }, bookingDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Find booking details by booking reference
        /// Note: Dates in response are in DD/MM/YYYY format
        /// </summary>
        /// <param name="bookingNumber">Booking reference number</param>
        /// <returns>Booking details with dates in DD/MM/YYYY format</returns>
        [HttpGet("{bookingNumber}")]
        public async Task<ActionResult<BookingDto>> GetBooking(string bookingNumber)
        {
            if (string.IsNullOrWhiteSpace(bookingNumber))
            {
                return BadRequest("Booking number is required");
            }

            var booking = await _bookingService.GetBookingByNumberAsync(bookingNumber);

            if (booking == null)
            {
                return NotFound($"Booking with number '{bookingNumber}' not found");
            }

            return Ok(booking);
        }

        /// <summary>
        /// Get all bookings for a specific room
        /// Note: Dates in response are in DD/MM/YYYY format
        /// </summary>
        /// <param name="roomId">Room ID</param>
        /// <returns>List of bookings for the room with dates in DD/MM/YYYY format</returns>
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByRoom(int roomId)
        {
            var bookings = await _bookingService.GetBookingsByRoomIdAsync(roomId);
            return Ok(bookings);
        }

        /// <summary>
        /// Delete a booking (for testing purposes)
        /// </summary>
        /// <param name="bookingNumber">Booking reference number</param>
        /// <returns>Success message</returns>
        [HttpDelete("{bookingNumber}")]
        public async Task<ActionResult> DeleteBooking(string bookingNumber)
        {
            var deleted = await _bookingService.DeleteBookingAsync(bookingNumber);

            if (!deleted)
            {
                return NotFound($"Booking with number '{bookingNumber}' not found");
            }

            return Ok($"Booking '{bookingNumber}' has been deleted");
        }
    }
} 