using System.ComponentModel.DataAnnotations;

namespace WaracleTestAPI.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string BookingNumber { get; set; } = null!;
        
        [Required]
        public int RoomId { get; set; }
        
        [Required]
        public DateTime CheckInDate { get; set; }
        
        [Required]
        public DateTime CheckOutDate { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int GuestCount { get; set; }
        
        [Required]
        [StringLength(100)]
        public string GuestName { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual Room Room { get; set; } = null!;
    }
} 