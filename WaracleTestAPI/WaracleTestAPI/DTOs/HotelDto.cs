namespace WaracleTestAPI.DTOs
{
    public class HotelDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public List<RoomDto> Rooms { get; set; } = new();
    }

    public class HotelSearchDto
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
    }
} 