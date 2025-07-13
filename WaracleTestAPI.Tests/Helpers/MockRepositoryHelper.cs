using Moq;
using WaracleTestAPI.Models;
using WaracleTestAPI.Repositories;

namespace WaracleTestAPI.Tests.Helpers
{
    public static class MockRepositoryHelper
    {
        public static Mock<IHotelRepository> CreateHotelRepositoryMock()
        {
            var mock = new Mock<IHotelRepository>();
            
            // Setup common scenarios
            mock.Setup(x => x.GetAllHotelsAsync())
                .ReturnsAsync(TestDataBuilder.CreateHotels(3));
            
            mock.Setup(x => x.GetHotelByIdAsync(1))
                .ReturnsAsync(TestDataBuilder.CreateHotel(1));
            
            mock.Setup(x => x.GetHotelByIdAsync(999))
                .ReturnsAsync((Hotel?)null);
            
            mock.Setup(x => x.SearchHotelsByNameAsync("Test"))
                .ReturnsAsync(new List<Hotel> { TestDataBuilder.CreateHotel(1, "Test Hotel") });
            
            mock.Setup(x => x.SearchHotelsByNameAsync("NonExistent"))
                .ReturnsAsync(new List<Hotel>());
            
            mock.Setup(x => x.HotelExistsAsync(1))
                .ReturnsAsync(true);
            
            mock.Setup(x => x.HotelExistsAsync(999))
                .ReturnsAsync(false);
            
            return mock;
        }

        public static Mock<IRoomRepository> CreateRoomRepositoryMock()
        {
            var mock = new Mock<IRoomRepository>();
            
            // Setup common scenarios
            mock.Setup(x => x.GetRoomByIdAsync(1))
                .ReturnsAsync(TestDataBuilder.CreateRoom(1));
            
            mock.Setup(x => x.GetRoomByIdAsync(999))
                .ReturnsAsync((Room?)null);
            
            mock.Setup(x => x.GetRoomsByHotelIdAsync(1))
                .ReturnsAsync(TestDataBuilder.CreateRooms(3, 1));
            
            mock.Setup(x => x.GetRoomsByHotelIdAsync(999))
                .ReturnsAsync(new List<Room>());
            
            mock.Setup(x => x.GetAvailableRoomsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(TestDataBuilder.CreateRooms(2));
            
            mock.Setup(x => x.RoomExistsAsync(1))
                .ReturnsAsync(true);
            
            mock.Setup(x => x.RoomExistsAsync(999))
                .ReturnsAsync(false);
            
            return mock;
        }

        public static Mock<IBookingRepository> CreateBookingRepositoryMock()
        {
            var mock = new Mock<IBookingRepository>();
            
            // Setup common scenarios
            mock.Setup(x => x.GetBookingByNumberAsync("BK20240101001"))
                .ReturnsAsync(TestDataBuilder.CreateBooking(1, "BK20240101001"));
            
            mock.Setup(x => x.GetBookingByNumberAsync("NONEXISTENT"))
                .ReturnsAsync((Booking?)null);
            
            mock.Setup(x => x.GetBookingsByRoomIdAsync(1))
                .ReturnsAsync(TestDataBuilder.CreateBookings(2, 1));
            
            mock.Setup(x => x.GetBookingsByRoomIdAsync(999))
                .ReturnsAsync(new List<Booking>());
            
            mock.Setup(x => x.HasOverlappingBookingAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int?>()))
                .ReturnsAsync(false);
            
            mock.Setup(x => x.BookingExistsAsync("BK20240101001"))
                .ReturnsAsync(true);
            
            mock.Setup(x => x.BookingExistsAsync("NONEXISTENT"))
                .ReturnsAsync(false);
            
            mock.Setup(x => x.GenerateUniqueBookingNumberAsync())
                .ReturnsAsync("BK20240101001");
            
            mock.Setup(x => x.DeleteBookingByNumberAsync("BK20240101001"))
                .ReturnsAsync(true);
            
            mock.Setup(x => x.DeleteBookingByNumberAsync("NONEXISTENT"))
                .ReturnsAsync(false);
            
            return mock;
        }

        public static Mock<IDataRepository> CreateDataRepositoryMock()
        {
            var mock = new Mock<IDataRepository>();
            
            // Setup common scenarios
            mock.Setup(x => x.GetDatabaseStatsAsync())
                .ReturnsAsync((hotels: 7, rooms: 42, roomTypes: 3, bookings: 0));
            
            mock.Setup(x => x.HasDataAsync())
                .ReturnsAsync(true);
            
            return mock;
        }
    }
} 