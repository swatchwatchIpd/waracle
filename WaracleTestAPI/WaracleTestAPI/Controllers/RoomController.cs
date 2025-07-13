using Microsoft.AspNetCore.Mvc;
using WaracleTestAPI.DTOs;
using WaracleTestAPI.Services;

namespace WaracleTestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        /// <summary>
        /// Find available rooms between two dates for a given number of people
        /// </summary>
        /// <param name="checkInDate">Check-in date in DD/MM/YYYY format (e.g., 25/12/2024)</param>
        /// <param name="checkOutDate">Check-out date in DD/MM/YYYY format (e.g., 28/12/2024)</param>
        /// <param name="guestCount">Number of guests</param>
        /// <param name="hotelId">Optional hotel ID to filter by specific hotel</param>
        /// <returns>List of available rooms</returns>
        [HttpGet("availability")]
        public async Task<ActionResult<IEnumerable<AvailableRoomDto>>> GetAvailableRooms(
            [FromQuery] DateTime checkInDate,
            [FromQuery] DateTime checkOutDate,
            [FromQuery] int guestCount,
            [FromQuery] int? hotelId = null)
        {
            try
            {
                var availableRooms = await _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount, hotelId);

                if (!availableRooms.Any())
                {
                    return NotFound($"No available rooms found for {guestCount} guests between {checkInDate:dd/MM/yyyy} and {checkOutDate:dd/MM/yyyy}");
                }

                return Ok(availableRooms);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get room details by ID
        /// </summary>
        /// <param name="id">Room ID</param>
        /// <returns>Room details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetRoom(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);

            if (room == null)
            {
                return NotFound($"Room with ID {id} not found");
            }

            return Ok(room);
        }

        /// <summary>
        /// Get all rooms for a specific hotel
        /// </summary>
        /// <param name="hotelId">Hotel ID</param>
        /// <returns>List of rooms in the hotel</returns>
        [HttpGet("by-hotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByHotel(int hotelId)
        {
            var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId);

            if (!rooms.Any())
            {
                return NotFound($"No rooms found for hotel with ID {hotelId}");
            }

            return Ok(rooms);
        }
    }
} 