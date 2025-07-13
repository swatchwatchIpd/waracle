using System.ComponentModel.DataAnnotations;

namespace WaracleTestAPI.Models
{
    public class Hotel
    {
        public int HotelId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [StringLength(255)]
        public string? Address { get; set; }
        
        // Navigation property
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
} 