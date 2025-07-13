using FluentAssertions;
using Moq;
using WaracleTestAPI.Models;
using WaracleTestAPI.Repositories;
using WaracleTestAPI.Services;
using WaracleTestAPI.Tests.Helpers;
using Xunit;

namespace WaracleTestAPI.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepositoryMock;
        private readonly Mock<IRoomRepository> _roomRepositoryMock;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _bookingRepositoryMock = MockRepositoryHelper.CreateBookingRepositoryMock();
            _roomRepositoryMock = MockRepositoryHelper.CreateRoomRepositoryMock();
            _bookingService = new BookingService(_bookingRepositoryMock.Object, _roomRepositoryMock.Object);
        }

        #region CreateBookingAsync Tests

        [Fact]
        public async Task CreateBookingAsync_ShouldCreateBooking_WhenValidData()
        {
            // Arrange
            var createBookingDto = TestDataBuilder.CreateBookingDto(
                roomId: 1,
                checkInDate: TestDataBuilder.TestDates.CheckIn1,  // 25/12/2024
                checkOutDate: TestDataBuilder.TestDates.CheckOut1, // 28/12/2024
                guestCount: 2,
                guestName: "John Doe"
            );

            var room = TestDataBuilder.CreateRoom(1, 1, 1, "101");
            room.RoomType = TestDataBuilder.CreateRoomType(1, "Double", 2);

            var createdBooking = TestDataBuilder.CreateBooking(1, "BK20240101001", 1, 
                checkInDate: TestDataBuilder.TestDates.CheckIn1, checkOutDate: TestDataBuilder.TestDates.CheckOut1,
                guestCount: 2, guestName: "John Doe");
            createdBooking.Room = room;
            
            _roomRepositoryMock.Setup(x => x.GetRoomByIdAsync(1))
                .ReturnsAsync(room);
            _bookingRepositoryMock.Setup(x => x.HasOverlappingBookingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(false);
            _bookingRepositoryMock.Setup(x => x.GenerateUniqueBookingNumberAsync())
                .ReturnsAsync("BK20240101001");
            _bookingRepositoryMock.Setup(x => x.CreateBookingAsync(It.IsAny<Booking>()))
                .ReturnsAsync(createdBooking);
            _bookingRepositoryMock.Setup(x => x.GetBookingByNumberAsync("BK20240101001"))
                .ReturnsAsync(createdBooking);

            // Act
            var result = await _bookingService.CreateBookingAsync(createBookingDto);

            // Assert
            result.Should().NotBeNull();
            result.BookingNumber.Should().Be("BK20240101001");
            result.RoomId.Should().Be(1);
            result.GuestName.Should().Be("John Doe");
            result.GuestCount.Should().Be(2);
            
            _roomRepositoryMock.Verify(x => x.GetRoomByIdAsync(1), Times.Once);
            _bookingRepositoryMock.Verify(x => x.HasOverlappingBookingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null), Times.Once);
            _bookingRepositoryMock.Verify(x => x.CreateBookingAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowArgumentException_WhenCheckInDateIsAfterCheckOutDate()
        {
            // Arrange
            var createBookingDto = TestDataBuilder.CreateBookingDto(
                checkInDate: DateTime.Today.AddDays(3),
                checkOutDate: DateTime.Today.AddDays(1)
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.CreateBookingAsync(createBookingDto));
            exception.Message.Should().Be("Check-in date must be before check-out date");
            
            _roomRepositoryMock.Verify(x => x.GetRoomByIdAsync(It.IsAny<int>()), Times.Never);
            _bookingRepositoryMock.Verify(x => x.CreateBookingAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowArgumentException_WhenCheckInDateIsInPast()
        {
            // Arrange
            var createBookingDto = TestDataBuilder.CreateBookingDto(
                checkInDate: DateTime.Today.AddDays(-1),
                checkOutDate: DateTime.Today.AddDays(1)
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.CreateBookingAsync(createBookingDto));
            exception.Message.Should().Be("Check-in date cannot be in the past");
            
            _roomRepositoryMock.Verify(x => x.GetRoomByIdAsync(It.IsAny<int>()), Times.Never);
            _bookingRepositoryMock.Verify(x => x.CreateBookingAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowArgumentException_WhenRoomDoesNotExist()
        {
            // Arrange
            var createBookingDto = TestDataBuilder.CreateBookingDto(roomId: 999);
            _roomRepositoryMock.Setup(x => x.GetRoomByIdAsync(999))
                .ReturnsAsync((Room?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.CreateBookingAsync(createBookingDto));
            exception.Message.Should().Be("Room with ID 999 not found");
            
            _roomRepositoryMock.Verify(x => x.GetRoomByIdAsync(999), Times.Once);
            _bookingRepositoryMock.Verify(x => x.CreateBookingAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowArgumentException_WhenGuestCountExceedsRoomCapacity()
        {
            // Arrange
            var createBookingDto = TestDataBuilder.CreateBookingDto(guestCount: 3);
            var room = TestDataBuilder.CreateRoom(1, 1, 1, "101");
            room.RoomType = TestDataBuilder.CreateRoomType(1, "Single", 1); // Capacity 1, but requesting 3 guests
            
            _roomRepositoryMock.Setup(x => x.GetRoomByIdAsync(1))
                .ReturnsAsync(room);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.CreateBookingAsync(createBookingDto));
            exception.Message.Should().Be("Guest count (3) exceeds room capacity (1)");
            
            _roomRepositoryMock.Verify(x => x.GetRoomByIdAsync(1), Times.Once);
            _bookingRepositoryMock.Verify(x => x.CreateBookingAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowInvalidOperationException_WhenRoomHasOverlappingBooking()
        {
            // Arrange
            var createBookingDto = TestDataBuilder.CreateBookingDto();
            var room = TestDataBuilder.CreateRoom(1, 1, 1, "101");
            room.RoomType = TestDataBuilder.CreateRoomType(1, "Double", 2);
            
            _roomRepositoryMock.Setup(x => x.GetRoomByIdAsync(1))
                .ReturnsAsync(room);
            _bookingRepositoryMock.Setup(x => x.HasOverlappingBookingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.CreateBookingAsync(createBookingDto));
            exception.Message.Should().Be("Room is already booked during the selected dates");
            
            _roomRepositoryMock.Verify(x => x.GetRoomByIdAsync(1), Times.Once);
            _bookingRepositoryMock.Verify(x => x.HasOverlappingBookingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null), Times.Once);
            _bookingRepositoryMock.Verify(x => x.CreateBookingAsync(It.IsAny<Booking>()), Times.Never);
        }

        [Theory]
        [InlineData(1, 1)] // Single room, 1 guest
        [InlineData(2, 2)] // Double room, 2 guests
        [InlineData(4, 4)] // Deluxe room, 4 guests
        public async Task CreateBookingAsync_ShouldCreateBooking_WhenGuestCountMatchesRoomCapacity(int capacity, int guestCount)
        {
            // Arrange
            var createBookingDto = TestDataBuilder.CreateBookingDto(guestCount: guestCount);
            var room = TestDataBuilder.CreateRoom(1, 1, 1, "101");
            room.RoomType = TestDataBuilder.CreateRoomType(1, "TestRoom", capacity);
            
            var createdBooking = TestDataBuilder.CreateBooking(1, "BK20240101001", 1, guestCount: guestCount);
            
            _roomRepositoryMock.Setup(x => x.GetRoomByIdAsync(1))
                .ReturnsAsync(room);
            _bookingRepositoryMock.Setup(x => x.HasOverlappingBookingAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null))
                .ReturnsAsync(false);
            _bookingRepositoryMock.Setup(x => x.CreateBookingAsync(It.IsAny<Booking>()))
                .ReturnsAsync(createdBooking);
            _bookingRepositoryMock.Setup(x => x.GetBookingByNumberAsync("BK20240101001"))
                .ReturnsAsync(createdBooking);

            // Act
            var result = await _bookingService.CreateBookingAsync(createBookingDto);

            // Assert
            result.Should().NotBeNull();
            result.GuestCount.Should().Be(guestCount);
            _bookingRepositoryMock.Verify(x => x.CreateBookingAsync(It.IsAny<Booking>()), Times.Once);
        }

        #endregion

        #region GetBookingByNumberAsync Tests

        [Fact]
        public async Task GetBookingByNumberAsync_ShouldReturnBooking_WhenBookingExists()
        {
            // Arrange
            var bookingNumber = "BK20240101001";
            var expectedBooking = TestDataBuilder.CreateBooking(1, bookingNumber, 1);
            _bookingRepositoryMock.Setup(x => x.GetBookingByNumberAsync(bookingNumber))
                .ReturnsAsync(expectedBooking);

            // Act
            var result = await _bookingService.GetBookingByNumberAsync(bookingNumber);

            // Assert
            result.Should().NotBeNull();
            result!.BookingNumber.Should().Be(bookingNumber);
            result.BookingId.Should().Be(1);
            _bookingRepositoryMock.Verify(x => x.GetBookingByNumberAsync(bookingNumber), Times.Once);
        }

        [Fact]
        public async Task GetBookingByNumberAsync_ShouldReturnNull_WhenBookingDoesNotExist()
        {
            // Arrange
            var bookingNumber = "NONEXISTENT";
            _bookingRepositoryMock.Setup(x => x.GetBookingByNumberAsync(bookingNumber))
                .ReturnsAsync((Booking?)null);

            // Act
            var result = await _bookingService.GetBookingByNumberAsync(bookingNumber);

            // Assert
            result.Should().BeNull();
            _bookingRepositoryMock.Verify(x => x.GetBookingByNumberAsync(bookingNumber), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        public async Task GetBookingByNumberAsync_ShouldReturnNull_WhenBookingNumberIsNullOrWhitespace(string? invalidBookingNumber)
        {
            // Act
            var result = await _bookingService.GetBookingByNumberAsync(invalidBookingNumber!);

            // Assert
            result.Should().BeNull();
            _bookingRepositoryMock.Verify(x => x.GetBookingByNumberAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region GetBookingsByRoomIdAsync Tests

        [Fact]
        public async Task GetBookingsByRoomIdAsync_ShouldReturnBookings_WhenBookingsExist()
        {
            // Arrange
            var roomId = 1;
            var expectedBookings = TestDataBuilder.CreateBookings(3, roomId);
            _bookingRepositoryMock.Setup(x => x.GetBookingsByRoomIdAsync(roomId))
                .ReturnsAsync(expectedBookings);

            // Act
            var result = await _bookingService.GetBookingsByRoomIdAsync(roomId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.All(b => b.RoomId == roomId).Should().BeTrue();
            _bookingRepositoryMock.Verify(x => x.GetBookingsByRoomIdAsync(roomId), Times.Once);
        }

        [Fact]
        public async Task GetBookingsByRoomIdAsync_ShouldReturnEmptyList_WhenNoBookingsExist()
        {
            // Arrange
            var roomId = 999;
            _bookingRepositoryMock.Setup(x => x.GetBookingsByRoomIdAsync(roomId))
                .ReturnsAsync(new List<Booking>());

            // Act
            var result = await _bookingService.GetBookingsByRoomIdAsync(roomId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _bookingRepositoryMock.Verify(x => x.GetBookingsByRoomIdAsync(roomId), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        public async Task GetBookingsByRoomIdAsync_ShouldReturnEmptyList_WhenRoomIdIsInvalid(int invalidRoomId)
        {
            // Arrange
            _bookingRepositoryMock.Setup(x => x.GetBookingsByRoomIdAsync(invalidRoomId))
                .ReturnsAsync(new List<Models.Booking>());

            // Act
            var result = await _bookingService.GetBookingsByRoomIdAsync(invalidRoomId);

            // Assert
            result.Should().BeEmpty();
            _bookingRepositoryMock.Verify(x => x.GetBookingsByRoomIdAsync(invalidRoomId), Times.Once);
        }

        #endregion

        #region DeleteBookingAsync Tests

        [Fact]
        public async Task DeleteBookingAsync_ShouldReturnTrue_WhenBookingExists()
        {
            // Arrange
            _bookingRepositoryMock.Setup(x => x.DeleteBookingByNumberAsync("BK001"))
                .ReturnsAsync(true);

            // Act
            var result = await _bookingService.DeleteBookingAsync("BK001");

            // Assert
            result.Should().BeTrue();
            _bookingRepositoryMock.Verify(x => x.DeleteBookingByNumberAsync("BK001"), Times.Once);
        }

        [Fact]
        public async Task DeleteBookingAsync_ShouldReturnFalse_WhenBookingDoesNotExist()
        {
            // Arrange
            _bookingRepositoryMock.Setup(x => x.DeleteBookingByNumberAsync("INVALID"))
                .ReturnsAsync(false);

            // Act
            var result = await _bookingService.DeleteBookingAsync("INVALID");

            // Assert
            result.Should().BeFalse();
            _bookingRepositoryMock.Verify(x => x.DeleteBookingByNumberAsync("INVALID"), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        public async Task DeleteBookingAsync_ShouldReturnFalse_WhenBookingNumberIsNullOrWhitespace(string? invalidBookingNumber)
        {
            // Act
            var result = await _bookingService.DeleteBookingAsync(invalidBookingNumber!);

            // Assert
            result.Should().BeFalse();
            _bookingRepositoryMock.Verify(x => x.DeleteBookingByNumberAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region Edge Cases and Error Scenarios

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowArgumentException_WhenCheckInDateEqualsCheckOutDate()
        {
            // Arrange
            var sameDate = DateTime.Today.AddDays(1);
            var createBookingDto = TestDataBuilder.CreateBookingDto(
                checkInDate: sameDate,
                checkOutDate: sameDate
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.CreateBookingAsync(createBookingDto));
            exception.Message.Should().Be("Check-in date must be before check-out date");
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldGenerateUniqueBookingNumber_WhenCreatingBooking()
        {
            // Arrange
            var createBookingDto = TestDataBuilder.CreateBookingDto();
            var room = TestDataBuilder.CreateRoomWithType(1, 1, 1, "101", "Single", 1);
            var expectedBooking = TestDataBuilder.CreateBooking(1, $"BK{DateTime.Now:yyyyMMdd}0001", 1, DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), 1, "John Doe");

            _roomRepositoryMock.Setup(x => x.GetRoomByIdAsync(1))
                .ReturnsAsync(room);
            _bookingRepositoryMock.Setup(x => x.HasOverlappingBookingAsync(1, createBookingDto.CheckInDate, createBookingDto.CheckOutDate, null))
                .ReturnsAsync(false);
            _bookingRepositoryMock.Setup(x => x.BookingExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _bookingRepositoryMock.Setup(x => x.CreateBookingAsync(It.IsAny<Models.Booking>()))
                .ReturnsAsync(expectedBooking);
            _bookingRepositoryMock.Setup(x => x.GetBookingByNumberAsync(It.IsAny<string>()))
                .ReturnsAsync(expectedBooking);

            // Act
            var result = await _bookingService.CreateBookingAsync(createBookingDto);

            // Assert
            result.Should().NotBeNull();
            result.BookingNumber.Should().NotBeNullOrEmpty();
            result.BookingNumber.Should().StartWith("BK");
            _bookingRepositoryMock.Verify(x => x.BookingExistsAsync(It.IsAny<string>()), Times.AtLeastOnce);
        }

        #endregion
    }
} 