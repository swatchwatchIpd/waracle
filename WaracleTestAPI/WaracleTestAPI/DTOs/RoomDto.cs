namespace WaracleTestAPI.DTOs
{
    public class RoomDto
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = null!;
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = null!;
        public int Capacity { get; set; }
        public string RoomNumber { get; set; } = null!;
    }

    public class AvailableRoomDto
    {
        public int RoomId { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = null!;
        public string HotelAddress { get; set; } = null!;
        public string RoomNumber { get; set; } = null!;
        public string RoomTypeName { get; set; } = null!;
        public int Capacity { get; set; }
    }

    public class RoomSearchDto
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int GuestCount { get; set; }
        public int? HotelId { get; set; }
    }
} 