using FluentAssertions;
using Moq;
using WaracleTestAPI.Repositories;
using WaracleTestAPI.Services;
using WaracleTestAPI.Tests.Helpers;
using Xunit;

namespace WaracleTestAPI.Tests.Services
{
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> _roomRepositoryMock;
        private readonly RoomService _roomService;

        public RoomServiceTests()
        {
            _roomRepositoryMock = MockRepositoryHelper.CreateRoomRepositoryMock();
            _roomService = new RoomService(_roomRepositoryMock.Object);
        }

        #region GetAvailableRoomsAsync Tests

        [Fact]
        public async Task GetAvailableRoomsAsync_ShouldReturnAvailableRooms_WhenValidParameters()
        {
            // Arrange
            var checkInDate = TestDataBuilder.TestDates.CheckIn1;  // 25/12/2024
            var checkOutDate = TestDataBuilder.TestDates.CheckOut1; // 28/12/2024
            var guestCount = 2;
            var hotelId = 1;

            var expectedRooms = TestDataBuilder.CreateRooms(2, hotelId);
            _roomRepositoryMock.Setup(x => x.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount, hotelId))
                .ReturnsAsync(expectedRooms);

            // Act
            var result = await _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount, hotelId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.All(r => r.HotelId == hotelId).Should().BeTrue();
            _roomRepositoryMock.Verify(x => x.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount, hotelId), Times.Once);
        }

        [Fact]
        public async Task GetAvailableRoomsAsync_ShouldReturnAvailableRooms_WhenHotelIdIsNull()
        {
            // Arrange
            var checkInDate = TestDataBuilder.TestDates.CheckIn1;  // 25/12/2024
            var checkOutDate = TestDataBuilder.TestDates.CheckOut1; // 28/12/2024
            var guestCount = 2;

            var expectedRooms = TestDataBuilder.CreateRooms(3);
            _roomRepositoryMock.Setup(x => x.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount, null))
                .ReturnsAsync(expectedRooms);

            // Act
            var result = await _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            _roomRepositoryMock.Verify(x => x.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount, null), Times.Once);
        }

        [Fact]
        public async Task GetAvailableRoomsAsync_ShouldReturnEmptyList_WhenNoRoomsAvailable()
        {
            // Arrange
            var checkInDate = TestDataBuilder.TestDates.CheckIn1;  // 25/12/2024
            var checkOutDate = TestDataBuilder.TestDates.CheckOut1; // 28/12/2024
            var guestCount = 2;

            _roomRepositoryMock.Setup(x => x.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount, null))
                .ReturnsAsync(new List<Models.Room>());

            // Act
            var result = await _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _roomRepositoryMock.Verify(x => x.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount, null), Times.Once);
        }

        [Fact]
        public async Task GetAvailableRoomsAsync_ShouldThrowArgumentException_WhenCheckInDateIsAfterCheckOutDate()
        {
            // Arrange
            var checkInDate = TestDataBuilder.TestDates.CheckOut1; // 28/12/2024
            var checkOutDate = TestDataBuilder.TestDates.CheckIn1;  // 25/12/2024 (earlier date)
            var guestCount = 2;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount));
            exception.Message.Should().Be("Check-in date must be before check-out date");
            
            _roomRepositoryMock.Verify(x => x.GetAvailableRoomsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public async Task GetAvailableRoomsAsync_ShouldThrowArgumentException_WhenCheckInDateIsInPast()
        {
            // Arrange
            var checkInDate = TestDataBuilder.TestDates.PastDate;    // 01/01/2024 (past date)
            var checkOutDate = TestDataBuilder.TestDates.CheckOut1;  // 28/12/2024
            var guestCount = 2;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount));
            exception.Message.Should().Be("Check-in date cannot be in the past");
            
            _roomRepositoryMock.Verify(x => x.GetAvailableRoomsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int?>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        public async Task GetAvailableRoomsAsync_ShouldThrowArgumentException_WhenGuestCountIsInvalid(int invalidGuestCount)
        {
            // Arrange & Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _roomService.GetAvailableRoomsAsync(DateTime.Today.AddDays(1), DateTime.Today.AddDays(3), invalidGuestCount));
            exception.Message.Should().Be("Guest count must be at least 1");
        }

        [Fact]
        public async Task GetAvailableRoomsAsync_ShouldThrowArgumentException_WhenCheckInDateEqualsCheckOutDate()
        {
            // Arrange & Act & Assert
            var sameDate = DateTime.Today.AddDays(1);
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _roomService.GetAvailableRoomsAsync(sameDate, sameDate, 2));
            exception.Message.Should().Be("Check-in date must be before check-out date");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(10)]
        public async Task GetAvailableRoomsAsync_ShouldAcceptValidGuestCounts(int validGuestCount)
        {
            // Arrange
            var checkInDate = DateTime.Today.AddDays(1);
            var checkOutDate = DateTime.Today.AddDays(3);
            var expectedRooms = new List<Models.Room>
            {
                TestDataBuilder.CreateRoomWithType(1, 1, 1, "101", "Single", validGuestCount >= 1 ? validGuestCount : 1)
            };

            _roomRepositoryMock.Setup(x => x.GetAvailableRoomsAsync(checkInDate, checkOutDate, validGuestCount, null))
                .ReturnsAsync(expectedRooms);

            // Act
            var result = await _roomService.GetAvailableRoomsAsync(checkInDate, checkOutDate, validGuestCount);

            // Assert
            result.Should().NotBeNull();
            _roomRepositoryMock.Verify(x => x.GetAvailableRoomsAsync(checkInDate, checkOutDate, validGuestCount, null), Times.Once);
        }

        #endregion

        #region GetRoomByIdAsync Tests

        [Fact]
        public async Task GetRoomByIdAsync_ShouldReturnRoom_WhenRoomExists()
        {
            // Arrange
            var expectedRoom = TestDataBuilder.CreateRoomWithType(1, 1, 1, "101", "Single", 1);
            _roomRepositoryMock.Setup(x => x.GetRoomByIdAsync(1))
                .ReturnsAsync(expectedRoom);

            // Act
            var result = await _roomService.GetRoomByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.RoomId.Should().Be(1);
            result.RoomNumber.Should().Be("101");
            result.RoomTypeName.Should().Be("Single");
            _roomRepositoryMock.Verify(x => x.GetRoomByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetRoomByIdAsync_ShouldReturnNull_WhenRoomDoesNotExist()
        {
            // Arrange
            _roomRepositoryMock.Setup(x => x.GetRoomByIdAsync(999))
                .ReturnsAsync((Models.Room?)null);

            // Act
            var result = await _roomService.GetRoomByIdAsync(999);

            // Assert
            result.Should().BeNull();
            _roomRepositoryMock.Verify(x => x.GetRoomByIdAsync(999), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        public async Task GetRoomByIdAsync_ShouldReturnNull_WhenRoomIdIsInvalid(int invalidRoomId)
        {
            // Arrange
            _roomRepositoryMock.Setup(x => x.GetRoomByIdAsync(invalidRoomId))
                .ReturnsAsync((Models.Room?)null);

            // Act
            var result = await _roomService.GetRoomByIdAsync(invalidRoomId);

            // Assert
            result.Should().BeNull();
            _roomRepositoryMock.Verify(x => x.GetRoomByIdAsync(invalidRoomId), Times.Once);
        }

        #endregion

        #region GetRoomsByHotelIdAsync Tests

        [Fact]
        public async Task GetRoomsByHotelIdAsync_ShouldReturnRooms_WhenHotelHasRooms()
        {
            // Arrange
            var expectedRooms = new List<Models.Room>
            {
                TestDataBuilder.CreateRoomWithType(1, 1, 1, "101", "Single", 1),
                TestDataBuilder.CreateRoomWithType(2, 1, 2, "102", "Double", 2)
            };
            _roomRepositoryMock.Setup(x => x.GetRoomsByHotelIdAsync(1))
                .ReturnsAsync(expectedRooms);

            // Act
            var result = await _roomService.GetRoomsByHotelIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(room => room.HotelName.Should().NotBeNullOrEmpty());
            _roomRepositoryMock.Verify(x => x.GetRoomsByHotelIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetRoomsByHotelIdAsync_ShouldReturnEmptyList_WhenHotelHasNoRooms()
        {
            // Arrange
            _roomRepositoryMock.Setup(x => x.GetRoomsByHotelIdAsync(999))
                .ReturnsAsync(new List<Models.Room>());

            // Act
            var result = await _roomService.GetRoomsByHotelIdAsync(999);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _roomRepositoryMock.Verify(x => x.GetRoomsByHotelIdAsync(999), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        public async Task GetRoomsByHotelIdAsync_ShouldReturnEmptyList_WhenHotelIdIsInvalid(int invalidHotelId)
        {
            // Arrange
            _roomRepositoryMock.Setup(x => x.GetRoomsByHotelIdAsync(invalidHotelId))
                .ReturnsAsync(new List<Models.Room>());

            // Act
            var result = await _roomService.GetRoomsByHotelIdAsync(invalidHotelId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _roomRepositoryMock.Verify(x => x.GetRoomsByHotelIdAsync(invalidHotelId), Times.Once);
        }

        #endregion
    }
} 