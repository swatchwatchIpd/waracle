using WaracleTestAPI.DTOs;
using WaracleTestAPI.Models;

namespace WaracleTestAPI.Utilities
{
    public static class MappingExtensions
    {
        // Hotel mappings
        public static HotelDto ToDto(this Hotel hotel)
        {
            return new HotelDto
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address,
                Rooms = hotel.Rooms?.Select(r => r.ToDto()).ToList() ?? new List<RoomDto>()
            };
        }

        public static HotelSearchDto ToSearchDto(this Hotel hotel)
        {
            return new HotelSearchDto
            {
                HotelId = hotel.HotelId,
                Name = hotel.Name,
                Address = hotel.Address
            };
        }

        // Room mappings
        public static RoomDto ToDto(this Room room)
        {
            return new RoomDto
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                HotelName = room.Hotel?.Name ?? "",
                RoomTypeId = room.RoomTypeId,
                RoomTypeName = room.RoomType?.Name ?? "",
                Capacity = room.RoomType?.Capacity ?? 0,
                RoomNumber = room.RoomNumber
            };
        }

        public static AvailableRoomDto ToAvailableRoomDto(this Room room)
        {
            return new AvailableRoomDto
            {
                RoomId = room.RoomId,
                HotelId = room.HotelId,
                HotelName = room.Hotel?.Name ?? "",
                HotelAddress = room.Hotel?.Address ?? "",
                RoomNumber = room.RoomNumber,
                RoomTypeName = room.RoomType?.Name ?? "",
                Capacity = room.RoomType?.Capacity ?? 0
            };
        }

        // Booking mappings
        public static BookingDto ToDto(this Booking booking)
        {
            return new BookingDto
            {
                BookingId = booking.BookingId,
                BookingNumber = booking.BookingNumber,
                RoomId = booking.RoomId,
                RoomNumber = booking.Room?.RoomNumber ?? "",
                HotelName = booking.Room?.Hotel?.Name ?? "",
                RoomTypeName = booking.Room?.RoomType?.Name ?? "",
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                GuestCount = booking.GuestCount,
                GuestName = booking.GuestName,
                CreatedAt = booking.CreatedAt
            };
        }

        public static Booking ToModel(this CreateBookingDto dto)
        {
            return new Booking
            {
                RoomId = dto.RoomId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                GuestCount = dto.GuestCount,
                GuestName = dto.GuestName,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Collection mappings
        public static IEnumerable<HotelSearchDto> ToSearchDtos(this IEnumerable<Hotel> hotels)
        {
            return hotels.Select(h => h.ToSearchDto());
        }

        public static IEnumerable<RoomDto> ToRoomDtos(this IEnumerable<Room> rooms)
        {
            return rooms.Select(r => r.ToDto());
        }

        public static IEnumerable<AvailableRoomDto> ToAvailableRoomDtos(this IEnumerable<Room> rooms)
        {
            return rooms.Select(r => r.ToAvailableRoomDto());
        }

        public static IEnumerable<BookingDto> ToBookingDtos(this IEnumerable<Booking> bookings)
        {
            return bookings.Select(b => b.ToDto());
        }
    }
} 