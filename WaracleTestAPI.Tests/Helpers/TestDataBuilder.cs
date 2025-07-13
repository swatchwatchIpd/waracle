using WaracleTestAPI.DTOs;
using WaracleTestAPI.Models;
using System.Globalization;

namespace WaracleTestAPI.Tests.Helpers
{
    public static class TestDataBuilder
    {
        // Date format constants
        private const string DateFormat = "dd/MM/yyyy";

        /// <summary>
        /// Creates a date from DD/MM/YYYY string format for testing
        /// </summary>
        /// <param name="dateString">Date in DD/MM/YYYY format (e.g., "25/12/2024")</param>
        /// <returns>DateTime object</returns>
        public static DateTime CreateDate(string dateString)
        {
            if (DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            throw new ArgumentException($"Invalid date format. Expected DD/MM/YYYY, got: {dateString}");
        }

        /// <summary>
        /// Gets future dates for testing in DD/MM/YYYY format
        /// </summary>
        public static class TestDates
        {
            public static DateTime CheckIn1 => CreateDate("25/12/2025");    // Future date
            public static DateTime CheckOut1 => CreateDate("28/12/2025");   // Future date
            public static DateTime CheckIn2 => CreateDate("30/12/2025");    // Future date
            public static DateTime CheckOut2 => CreateDate("02/01/2026");   // Future date
            public static DateTime CheckIn3 => CreateDate("05/01/2026");    // Future date
            public static DateTime CheckOut3 => CreateDate("08/01/2026");   // Future date
            public static DateTime CheckIn4 => CreateDate("10/01/2026");    // Future date
            public static DateTime CheckOut4 => CreateDate("13/01/2026");   // Future date
            public static DateTime PastDate => CreateDate("01/01/2024");    // Past date for negative testing
            public static DateTime TodayDate => CreateDate(DateTime.Today.ToString(DateFormat));
            public static DateTime TomorrowDate => CreateDate(DateTime.Today.AddDays(1).ToString(DateFormat));
            public static DateTime FutureCheckIn => CreateDate(DateTime.Today.AddDays(30).ToString(DateFormat));
            public static DateTime FutureCheckOut => CreateDate(DateTime.Today.AddDays(33).ToString(DateFormat));
        }

        public static Hotel CreateHotel(int id = 1, string name = "Test Hotel", string? address = "123 Test St")
        {
            return new Hotel
            {
                HotelId = id,
                Name = name,
                Address = address,
                Rooms = new List<Room>()
            };
        }

        public static Room CreateRoom(int id = 1, int hotelId = 1, int roomTypeId = 1, string roomNumber = "101")
        {
            return new Room
            {
                RoomId = id,
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                RoomNumber = roomNumber,
                Hotel = CreateHotel(hotelId),
                RoomType = CreateRoomType(roomTypeId)
            };
        }

        public static Room CreateRoomWithType(int id, int hotelId, int roomTypeId, string roomNumber, string roomTypeName, int capacity)
        {
            return new Room
            {
                RoomId = id,
                HotelId = hotelId,
                RoomTypeId = roomTypeId,
                RoomNumber = roomNumber,
                Hotel = CreateHotel(hotelId),
                RoomType = CreateRoomType(roomTypeId, roomTypeName, capacity)
            };
        }

        public static RoomType CreateRoomType(int id = 1, string name = "Single", int capacity = 1)
        {
            return new RoomType
            {
                RoomTypeId = id,
                Name = name,
                Capacity = capacity
            };
        }

        public static Booking CreateBooking(int id = 1, string bookingNumber = "BK20240101001", int roomId = 1, 
            DateTime? checkInDate = null, DateTime? checkOutDate = null, int guestCount = 1, string guestName = "John Doe")
        {
            return new Booking
            {
                BookingId = id,
                BookingNumber = bookingNumber,
                RoomId = roomId,
                CheckInDate = checkInDate ?? TestDates.CheckIn1,
                CheckOutDate = checkOutDate ?? TestDates.CheckOut1,
                GuestCount = guestCount,
                GuestName = guestName,
                CreatedAt = DateTime.UtcNow,
                Room = CreateRoom(roomId)
            };
        }

        public static CreateBookingDto CreateBookingDto(int roomId = 1, DateTime? checkInDate = null, 
            DateTime? checkOutDate = null, int guestCount = 1, string guestName = "John Doe")
        {
            return new CreateBookingDto
            {
                RoomId = roomId,
                CheckInDate = checkInDate ?? TestDates.CheckIn1,
                CheckOutDate = checkOutDate ?? TestDates.CheckOut1,
                GuestCount = guestCount,
                GuestName = guestName
            };
        }

        public static List<Hotel> CreateHotels(int count = 3)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateHotel(i, $"Hotel {i}", $"{i} Test Street"))
                .ToList();
        }

        public static List<Room> CreateRooms(int count = 3, int hotelId = 1)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateRoom(i, hotelId, i % 3 + 1, $"{hotelId}0{i}"))
                .ToList();
        }

        public static List<Booking> CreateBookings(int count = 3, int roomId = 1)
        {
            var testDates = new[]
            {
                (TestDates.CheckIn1, TestDates.CheckOut1),
                (TestDates.CheckIn2, TestDates.CheckOut2),
                (TestDates.CheckIn3, TestDates.CheckOut3)
            };

            return Enumerable.Range(1, count)
                .Select(i => 
                {
                    var (checkIn, checkOut) = testDates[(i - 1) % testDates.Length];
                    return CreateBooking(i, $"BK2024010{i:00}", roomId, checkIn, checkOut, 1, $"Guest {i}");
                })
                .ToList();
        }
    }
} 