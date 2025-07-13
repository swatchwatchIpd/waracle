using FluentAssertions;
using Moq;
using WaracleTestAPI.Repositories;
using WaracleTestAPI.Services;
using WaracleTestAPI.Tests.Helpers;
using Xunit;

namespace WaracleTestAPI.Tests.Services
{
    public class DataServiceTests
    {
        private readonly Mock<IDataRepository> _dataRepositoryMock;
        private readonly DataService _dataService;

        public DataServiceTests()
        {
            _dataRepositoryMock = MockRepositoryHelper.CreateDataRepositoryMock();
            _dataService = new DataService(_dataRepositoryMock.Object);
        }

        #region ResetDataAsync Tests

        [Fact]
        public async Task ResetDataAsync_ShouldCallRepositoryResetData()
        {
            // Arrange
            _dataRepositoryMock.Setup(x => x.ResetDataAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _dataService.ResetDataAsync();

            // Assert
            _dataRepositoryMock.Verify(x => x.ResetDataAsync(), Times.Once);
        }

        [Fact]
        public async Task ResetDataAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            _dataRepositoryMock.Setup(x => x.ResetDataAsync())
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _dataService.ResetDataAsync());
            _dataRepositoryMock.Verify(x => x.ResetDataAsync(), Times.Once);
        }

        [Fact]
        public async Task ResetDataAsync_ShouldNotThrowException_WhenRepositorySucceeds()
        {
            // Arrange
            _dataRepositoryMock.Setup(x => x.ResetDataAsync())
                .Returns(Task.CompletedTask);

            // Act
            var exception = await Record.ExceptionAsync(() => _dataService.ResetDataAsync());

            // Assert
            exception.Should().BeNull();
            _dataRepositoryMock.Verify(x => x.ResetDataAsync(), Times.Once);
        }

        #endregion

        #region SeedDataAsync Tests

        [Fact]
        public async Task SeedDataAsync_ShouldCallRepositorySeedData()
        {
            // Arrange
            _dataRepositoryMock.Setup(x => x.SeedDataAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _dataService.SeedDataAsync();

            // Assert
            _dataRepositoryMock.Verify(x => x.SeedDataAsync(), Times.Once);
        }

        [Fact]
        public async Task SeedDataAsync_ShouldHandleRepositoryException_WhenSeedingData()
        {
            // Arrange
            _dataRepositoryMock.Setup(x => x.SeedDataAsync())
                .ThrowsAsync(new Exception("Database seeding failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _dataService.SeedDataAsync());
            _dataRepositoryMock.Verify(x => x.SeedDataAsync(), Times.Once);
        }

        [Fact]
        public async Task SeedDataAsync_ShouldNotThrowException_WhenRepositorySucceeds()
        {
            // Arrange
            _dataRepositoryMock.Setup(x => x.SeedDataAsync())
                .Returns(Task.CompletedTask);

            // Act
            var exception = await Record.ExceptionAsync(() => _dataService.SeedDataAsync());

            // Assert
            exception.Should().BeNull();
            _dataRepositoryMock.Verify(x => x.SeedDataAsync(), Times.Once);
        }

        #endregion

        #region GetDatabaseStatsAsync Tests

        [Fact]
        public async Task GetDatabaseStatsAsync_ShouldReturnStats_WhenRepositorySucceeds()
        {
            // Arrange
            var expectedStats = (hotels: 7, rooms: 42, roomTypes: 3, bookings: 0);
            _dataRepositoryMock.Setup(x => x.GetDatabaseStatsAsync())
                .ReturnsAsync(expectedStats);

            // Act
            var result = await _dataService.GetDatabaseStatsAsync();

            // Assert
            result.Should().NotBeNull();
            var stats = result.GetType().GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(result));
            
            stats.Should().ContainKey("hotels");
            stats.Should().ContainKey("rooms");
            stats.Should().ContainKey("roomTypes");
            stats.Should().ContainKey("bookings");
            
            stats["hotels"].Should().Be(7);
            stats["rooms"].Should().Be(42);
            stats["roomTypes"].Should().Be(3);
            stats["bookings"].Should().Be(0);
            
            _dataRepositoryMock.Verify(x => x.GetDatabaseStatsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetDatabaseStatsAsync_ShouldReturnEmptyStats_WhenDatabaseIsEmpty()
        {
            // Arrange
            var emptyStats = (hotels: 0, rooms: 0, roomTypes: 0, bookings: 0);
            _dataRepositoryMock.Setup(x => x.GetDatabaseStatsAsync())
                .ReturnsAsync(emptyStats);

            // Act
            var result = await _dataService.GetDatabaseStatsAsync();

            // Assert
            result.Should().NotBeNull();
            var stats = result.GetType().GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(result));
            
            stats["hotels"].Should().Be(0);
            stats["rooms"].Should().Be(0);
            stats["roomTypes"].Should().Be(0);
            stats["bookings"].Should().Be(0);
            
            _dataRepositoryMock.Verify(x => x.GetDatabaseStatsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetDatabaseStatsAsync_ShouldReturnStatsWithBookings_WhenBookingsExist()
        {
            // Arrange
            var statsWithBookings = (hotels: 7, rooms: 42, roomTypes: 3, bookings: 15);
            _dataRepositoryMock.Setup(x => x.GetDatabaseStatsAsync())
                .ReturnsAsync(statsWithBookings);

            // Act
            var result = await _dataService.GetDatabaseStatsAsync();

            // Assert
            result.Should().NotBeNull();
            var stats = result.GetType().GetProperties()
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(result));
            
            stats["bookings"].Should().Be(15);
            _dataRepositoryMock.Verify(x => x.GetDatabaseStatsAsync(), Times.Once);
        }

        #endregion

        #region Edge Cases and Error Scenarios

        [Fact]
        public async Task SeedDataAsync_ShouldWork_AfterResetData()
        {
            // This test simulates the workflow: Reset -> Seed
            // Arrange
            _dataRepositoryMock.Setup(x => x.ResetDataAsync())
                .Returns(Task.CompletedTask);
            _dataRepositoryMock.Setup(x => x.SeedDataAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _dataService.ResetDataAsync();
            await _dataService.SeedDataAsync();

            // Assert
            _dataRepositoryMock.Verify(x => x.ResetDataAsync(), Times.Once);
            _dataRepositoryMock.Verify(x => x.SeedDataAsync(), Times.Once);
        }

        #endregion
    }
} 