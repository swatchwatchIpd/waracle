using Microsoft.AspNetCore.Mvc;
using WaracleTestAPI.Services;

namespace WaracleTestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Reset all data - removes all bookings, rooms, hotels, and room types
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("reset")]
        public async Task<ActionResult> ResetData()
        {
            try
            {
                await _dataService.ResetDataAsync();
                return Ok("All data has been reset successfully. Database is now empty.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error resetting data: {ex.Message}");
            }
        }

        /// <summary>
        /// Seed database with initial data for testing
        /// </summary>
        /// <returns>Success message with data counts</returns>
        [HttpPost("seed")]
        public async Task<ActionResult> SeedData()
        {
            try
            {
                await _dataService.SeedDataAsync();
                var stats = await _dataService.GetDatabaseStatsAsync();

                return Ok(new
                {
                    message = "Database has been seeded successfully",
                    data = stats
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error seeding data: {ex.Message}");
            }
        }

        /// <summary>
        /// Get database statistics
        /// </summary>
        /// <returns>Current data counts</returns>
        [HttpGet("stats")]
        public async Task<ActionResult> GetDatabaseStats()
        {
            try
            {
                var stats = await _dataService.GetDatabaseStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting database stats: {ex.Message}");
            }
        }


    }
} 