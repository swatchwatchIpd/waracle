using System.ComponentModel.DataAnnotations;

namespace WaracleTestAPI.Models
{
    public class RoomType
    {
        public int RoomTypeId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
        
        // Navigation property
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
} 