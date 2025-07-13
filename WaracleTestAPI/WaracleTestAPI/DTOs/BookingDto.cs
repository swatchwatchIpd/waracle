using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WaracleTestAPI.Utilities;

namespace WaracleTestAPI.DTOs
{
    public class BookingDto
    {
        [Required]
        public int BookingId { get; set; }
        
        [Required]
        [StringLength(50, ErrorMessage = "Booking number cannot exceed 50 characters")]
        public string BookingNumber { get; set; } = null!;
        
        [Required]
        public int RoomId { get; set; }
        
        [Required]
        [StringLength(10, ErrorMessage = "Room number cannot exceed 10 characters")]
        public string RoomNumber { get; set; } = null!;
        
        [Required]
        [StringLength(100, ErrorMessage = "Hotel name cannot exceed 100 characters")]
        public string HotelName { get; set; } = null!;
        
        [Required]
        [StringLength(50, ErrorMessage = "Room type name cannot exceed 50 characters")]
        public string RoomTypeName { get; set; } = null!;
        
        [Required]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime CheckInDate { get; set; }
        
        [Required]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime CheckOutDate { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Guest count must be at least 1")]
        public int GuestCount { get; set; }
        
        [Required]
        [StringLength(100, ErrorMessage = "Guest name cannot exceed 100 characters")]
        public string GuestName { get; set; } = null!;
        
        [Required]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBookingDto
    {
        [Required(ErrorMessage = "Room ID is required")]
        public int RoomId { get; set; }
        
        [Required(ErrorMessage = "Check-in date is required")]
        [JsonConverter(typeof(DateTimeConverter))]
        [Display(Name = "Check-in Date", Description = "Check-in date in DD/MM/YYYY format")]
        public DateTime CheckInDate { get; set; }
        
        [Required(ErrorMessage = "Check-out date is required")]
        [JsonConverter(typeof(DateTimeConverter))]
        [Display(Name = "Check-out Date", Description = "Check-out date in DD/MM/YYYY format")]
        public DateTime CheckOutDate { get; set; }
        
        [Required(ErrorMessage = "Guest count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Guest count must be at least 1")]
        public int GuestCount { get; set; }
        
        [Required(ErrorMessage = "Guest name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Guest name must be between 1 and 100 characters")]
        public string GuestName { get; set; } = null!;
    }

    public class BookingSearchDto
    {
        [Required(ErrorMessage = "Booking number is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Booking number must be between 1 and 50 characters")]
        public string BookingNumber { get; set; } = null!;
    }
} 