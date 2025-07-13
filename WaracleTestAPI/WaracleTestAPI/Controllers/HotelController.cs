using Microsoft.AspNetCore.Mvc;
using WaracleTestAPI.DTOs;
using WaracleTestAPI.Services;

namespace WaracleTestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        /// <summary>
        /// Find hotels by name (partial match)
        /// </summary>
        /// <param name="name">Hotel name to search for</param>
        /// <returns>List of matching hotels</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<HotelSearchDto>>> SearchHotels([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Hotel name is required");
            }

            var hotels = await _hotelService.SearchHotelsByNameAsync(name);

            if (!hotels.Any())
            {
                return NotFound($"No hotels found with name containing '{name}'");
            }

            return Ok(hotels);
        }

        /// <summary>
        /// Get all hotels
        /// </summary>
        /// <returns>List of all hotels</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelSearchDto>>> GetAllHotels()
        {
            var hotels = await _hotelService.GetAllHotelsAsync();
            return Ok(hotels);
        }

        /// <summary>
        /// Get a specific hotel by ID with its rooms
        /// </summary>
        /// <param name="id">Hotel ID</param>
        /// <returns>Hotel details with rooms</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);

            if (hotel == null)
            {
                return NotFound($"Hotel with ID {id} not found");
            }

            return Ok(hotel);
        }
    }
} 