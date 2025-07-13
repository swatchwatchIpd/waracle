using System.ComponentModel.DataAnnotations;

namespace WaracleTestAPI.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        
        [Required]
        public int HotelId { get; set; }
        
        [Required]
        public int RoomTypeId { get; set; }
        
        [Required]
        [StringLength(10)]
        public string RoomNumber { get; set; } = null!;
        
        // Navigation properties
        public virtual Hotel Hotel { get; set; } = null!;
        public virtual RoomType RoomType { get; set; } = null!;
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
} 