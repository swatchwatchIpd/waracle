using FluentAssertions;
using Moq;
using WaracleTestAPI.Repositories;
using WaracleTestAPI.Services;
using WaracleTestAPI.Tests.Helpers;
using Xunit;

namespace WaracleTestAPI.Tests.Services
{
    public class HotelServiceTests
    {
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;
        private readonly HotelService _hotelService;

        public HotelServiceTests()
        {
            _hotelRepositoryMock = MockRepositoryHelper.CreateHotelRepositoryMock();
            _hotelService = new HotelService(_hotelRepositoryMock.Object);
        }

        #region GetAllHotelsAsync Tests

        [Fact]
        public async Task GetAllHotelsAsync_ShouldReturnAllHotels_WhenHotelsExist()
        {
            // Arrange
            var expectedHotels = TestDataBuilder.CreateHotels(3);
            _hotelRepositoryMock.Setup(x => x.GetAllHotelsAsync())
                .ReturnsAsync(expectedHotels);

            // Act
            var result = await _hotelService.GetAllHotelsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.First().Name.Should().Be("Hotel 1");
            _hotelRepositoryMock.Verify(x => x.GetAllHotelsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllHotelsAsync_ShouldReturnEmptyList_WhenNoHotelsExist()
        {
            // Arrange
            _hotelRepositoryMock.Setup(x => x.GetAllHotelsAsync())
                .ReturnsAsync(new List<Models.Hotel>());

            // Act
            var result = await _hotelService.GetAllHotelsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _hotelRepositoryMock.Verify(x => x.GetAllHotelsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllHotelsAsync_ShouldReturnSearchDtos_WithCorrectMapping()
        {
            // Arrange
            var hotels = TestDataBuilder.CreateHotels(2);
            _hotelRepositoryMock.Setup(x => x.GetAllHotelsAsync())
                .ReturnsAsync(hotels);

            // Act
            var result = await _hotelService.GetAllHotelsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            
            var firstHotel = result.First();
            firstHotel.HotelId.Should().Be(1);
            firstHotel.Name.Should().Be("Hotel 1");
            firstHotel.Address.Should().Be("1 Test Street");
        }

        #endregion

        #region GetHotelByIdAsync Tests

        [Fact]
        public async Task GetHotelByIdAsync_ShouldReturnHotel_WhenHotelExists()
        {
            // Arrange
            var expectedHotel = TestDataBuilder.CreateHotel(1, "Test Hotel", "123 Test St");
            expectedHotel.Rooms = TestDataBuilder.CreateRooms(2, 1);
            _hotelRepositoryMock.Setup(x => x.GetHotelByIdAsync(1))
                .ReturnsAsync(expectedHotel);

            // Act
            var result = await _hotelService.GetHotelByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.HotelId.Should().Be(1);
            result.Name.Should().Be("Test Hotel");
            result.Address.Should().Be("123 Test St");
            result.Rooms.Should().HaveCount(2);
            _hotelRepositoryMock.Verify(x => x.GetHotelByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetHotelByIdAsync_ShouldReturnNull_WhenHotelDoesNotExist()
        {
            // Arrange
            _hotelRepositoryMock.Setup(x => x.GetHotelByIdAsync(999))
                .ReturnsAsync((Models.Hotel?)null);

            // Act
            var result = await _hotelService.GetHotelByIdAsync(999);

            // Assert
            result.Should().BeNull();
            _hotelRepositoryMock.Verify(x => x.GetHotelByIdAsync(999), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        public async Task GetHotelByIdAsync_ShouldReturnNull_WhenIdIsInvalid(int invalidId)
        {
            // Arrange
            _hotelRepositoryMock.Setup(x => x.GetHotelByIdAsync(invalidId))
                .ReturnsAsync((Models.Hotel?)null);

            // Act
            var result = await _hotelService.GetHotelByIdAsync(invalidId);

            // Assert
            result.Should().BeNull();
            _hotelRepositoryMock.Verify(x => x.GetHotelByIdAsync(invalidId), Times.Once);
        }

        #endregion

        #region SearchHotelsByNameAsync Tests

        [Fact]
        public async Task SearchHotelsByNameAsync_ShouldReturnMatchingHotels_WhenNameExists()
        {
            // Arrange
            var expectedHotels = new List<Models.Hotel>
            {
                TestDataBuilder.CreateHotel(1, "Grand Hotel", "123 Main St"),
                TestDataBuilder.CreateHotel(2, "Hotel Grand", "456 Oak Ave")
            };
            _hotelRepositoryMock.Setup(x => x.SearchHotelsByNameAsync("Grand"))
                .ReturnsAsync(expectedHotels);

            // Act
            var result = await _hotelService.SearchHotelsByNameAsync("Grand");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(hotel => hotel.Name.Should().Contain("Grand"));
            _hotelRepositoryMock.Verify(x => x.SearchHotelsByNameAsync("Grand"), Times.Once);
        }

        [Fact]
        public async Task SearchHotelsByNameAsync_ShouldReturnEmptyList_WhenNoMatchingHotels()
        {
            // Arrange
            _hotelRepositoryMock.Setup(x => x.SearchHotelsByNameAsync("NonexistentHotel"))
                .ReturnsAsync(new List<Models.Hotel>());

            // Act
            var result = await _hotelService.SearchHotelsByNameAsync("NonexistentHotel");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _hotelRepositoryMock.Verify(x => x.SearchHotelsByNameAsync("NonexistentHotel"), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        public async Task SearchHotelsByNameAsync_ShouldReturnEmptyList_WhenNameIsNullOrWhitespace(string? invalidName)
        {
            // Act
            var result = await _hotelService.SearchHotelsByNameAsync(invalidName!);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _hotelRepositoryMock.Verify(x => x.SearchHotelsByNameAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SearchHotelsByNameAsync_ShouldBeCaseInsensitive()
        {
            // Arrange
            var expectedHotels = new List<Models.Hotel>
            {
                TestDataBuilder.CreateHotel(1, "Grand Hotel", "123 Main St")
            };
            _hotelRepositoryMock.Setup(x => x.SearchHotelsByNameAsync("grand"))
                .ReturnsAsync(expectedHotels);

            // Act
            var result = await _hotelService.SearchHotelsByNameAsync("grand");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            _hotelRepositoryMock.Verify(x => x.SearchHotelsByNameAsync("grand"), Times.Once);
        }

        #endregion
    }
} 