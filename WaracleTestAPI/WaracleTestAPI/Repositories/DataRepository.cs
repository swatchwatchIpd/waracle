using Microsoft.EntityFrameworkCore;
using WaracleTestAPI.Data;
using WaracleTestAPI.Models;

namespace WaracleTestAPI.Repositories
{
    public class DataRepository : IDataRepository
    {
        private readonly HotelBookingDbContext _context;

        public DataRepository(HotelBookingDbContext context)
        {
            _context = context;
        }

        public async Task ResetDataAsync()
        {
            // Use raw SQL to truncate all tables in the correct order
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Booking");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Room");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM Hotel");
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM RoomType");
            
            // Reset identity columns to start from 1 again (SQL Server specific)
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Booking', RESEED, 0)");
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Room', RESEED, 0)");
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Hotel', RESEED, 0)");
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('RoomType', RESEED, 0)");
        }

        public async Task SeedDataAsync()
        {
            // Just insert the data - no checks, no EnsureCreated
            await SeedDataManually();
        }

        public async Task<(int hotels, int rooms, int roomTypes, int bookings)> GetDatabaseStatsAsync()
        {
            var hotels = await _context.Hotels.CountAsync();
            var rooms = await _context.Rooms.CountAsync();
            var roomTypes = await _context.RoomTypes.CountAsync();
            var bookings = await _context.Bookings.CountAsync();

            return (hotels, rooms, roomTypes, bookings);
        }

        public async Task<bool> HasDataAsync()
        {
            return await _context.Hotels.AnyAsync() || 
                   await _context.RoomTypes.AnyAsync() || 
                   await _context.Rooms.AnyAsync() || 
                   await _context.Bookings.AnyAsync();
        }

        private async Task SeedDataManually()
        {
            // Add RoomTypes
            var roomTypes = new[]
            {
                new RoomType { Name = "Single", Capacity = 1 },
                new RoomType { Name = "Double", Capacity = 2 },
                new RoomType { Name = "Deluxe", Capacity = 4 }
            };
            _context.RoomTypes.AddRange(roomTypes);
            await _context.SaveChangesAsync();

            // Add Hotels
            var hotels = new[]
            {
                new Hotel { Name = "Hotel Azure", Address = "1 Seaside Blvd" },
                new Hotel { Name = "Mountain Retreat", Address = "22 Hilltop Rd" },
                new Hotel { Name = "Urban Stay", Address = "100 Main St" },
                new Hotel { Name = "Grand Palace", Address = "9 Royal Ave" },
                new Hotel { Name = "Lakeside Inn", Address = "78 Lakeview Dr" },
                new Hotel { Name = "Skyview Hotel", Address = "200 Cloud St" },
                new Hotel { Name = "Sunset Villas", Address = "303 Sunset Blvd" }
            };
            _context.Hotels.AddRange(hotels);
            await _context.SaveChangesAsync();

            // Add Rooms (6 per hotel: 2 Single, 2 Double, 2 Deluxe)
            var rooms = new List<Room>();
            for (int hotelId = 1; hotelId <= 7; hotelId++)
            {
                // 2 Single rooms
                rooms.Add(new Room { HotelId = hotelId, RoomTypeId = 1, RoomNumber = $"{hotelId}01" });
                rooms.Add(new Room { HotelId = hotelId, RoomTypeId = 1, RoomNumber = $"{hotelId}02" });
                
                // 2 Double rooms
                rooms.Add(new Room { HotelId = hotelId, RoomTypeId = 2, RoomNumber = $"{hotelId}03" });
                rooms.Add(new Room { HotelId = hotelId, RoomTypeId = 2, RoomNumber = $"{hotelId}04" });
                
                // 2 Deluxe rooms
                rooms.Add(new Room { HotelId = hotelId, RoomTypeId = 3, RoomNumber = $"{hotelId}05" });
                rooms.Add(new Room { HotelId = hotelId, RoomTypeId = 3, RoomNumber = $"{hotelId}06" });
            }
            
            _context.Rooms.AddRange(rooms);
            await _context.SaveChangesAsync();

            // Add Bookings - Sample data for testing
            var bookings = new List<Booking>();
            
            // Past bookings (completed)
            bookings.AddRange(new[]
            {
                new Booking { BookingNumber = "BK001", RoomId = 1, CheckInDate = new DateTime(2024, 1, 15), CheckOutDate = new DateTime(2024, 1, 18), GuestCount = 1, GuestName = "John Smith", CreatedAt = new DateTime(2024, 1, 10, 10, 0, 0) },
                new Booking { BookingNumber = "BK002", RoomId = 8, CheckInDate = new DateTime(2024, 1, 20), CheckOutDate = new DateTime(2024, 1, 25), GuestCount = 2, GuestName = "Tyler White", CreatedAt = new DateTime(2024, 1, 15, 14, 30, 0) },
                new Booking { BookingNumber = "BK003", RoomId = 18, CheckInDate = new DateTime(2024, 2, 1), CheckOutDate = new DateTime(2024, 2, 5), GuestCount = 4, GuestName = "Bob Johnson", CreatedAt = new DateTime(2024, 1, 28, 9, 15, 0) }
            });

            // Current bookings (overlapping with common test dates)
            bookings.AddRange(new[]
            {
                new Booking { BookingNumber = "BK004", RoomId = 2, CheckInDate = new DateTime(2024, 12, 15), CheckOutDate = new DateTime(2024, 12, 20), GuestCount = 1, GuestName = "Zalika White", CreatedAt = new DateTime(2024, 12, 10, 16, 45, 0) },
                new Booking { BookingNumber = "BK005", RoomId = 9, CheckInDate = new DateTime(2024, 12, 18), CheckOutDate = new DateTime(2024, 12, 22), GuestCount = 2, GuestName = "Charlie Wilson", CreatedAt = new DateTime(2024, 12, 12, 11, 20, 0) },
                new Booking { BookingNumber = "BK006", RoomId = 24, CheckInDate = new DateTime(2024, 12, 20), CheckOutDate = new DateTime(2024, 12, 25), GuestCount = 3, GuestName = "Diana Miller", CreatedAt = new DateTime(2024, 12, 15, 13, 10, 0) }
            });

            // Future bookings (for testing availability)
            bookings.AddRange(new[]
            {
                new Booking { BookingNumber = "BK007", RoomId = 3, CheckInDate = new DateTime(2025, 1, 10), CheckOutDate = new DateTime(2025, 1, 15), GuestCount = 1, GuestName = "Eve Davis", CreatedAt = new DateTime(2024, 12, 18, 10, 30, 0) },
                new Booking { BookingNumber = "BK008", RoomId = 16, CheckInDate = new DateTime(2025, 1, 15), CheckOutDate = new DateTime(2025, 1, 20), GuestCount = 2, GuestName = "Frank Garcia", CreatedAt = new DateTime(2024, 12, 18, 15, 45, 0) },
                new Booking { BookingNumber = "BK009", RoomId = 29, CheckInDate = new DateTime(2025, 1, 20), CheckOutDate = new DateTime(2025, 1, 25), GuestCount = 4, GuestName = "Olivia White", CreatedAt = new DateTime(2024, 12, 18, 12, 15, 0) }
            });

            // Additional bookings across different hotels for comprehensive testing
            bookings.AddRange(new[]
            {
                new Booking { BookingNumber = "BK010", RoomId = 10, CheckInDate = new DateTime(2025, 2, 1), CheckOutDate = new DateTime(2025, 2, 5), GuestCount = 2, GuestName = "Christopher White", CreatedAt = new DateTime(2024, 12, 18, 14, 20, 0) },
                new Booking { BookingNumber = "BK011", RoomId = 12, CheckInDate = new DateTime(2025, 2, 10), CheckOutDate = new DateTime(2025, 2, 15), GuestCount = 3, GuestName = "Iris Taylor", CreatedAt = new DateTime(2024, 12, 18, 16, 30, 0) },
                new Booking { BookingNumber = "BK012", RoomId = 36, CheckInDate = new DateTime(2025, 2, 20), CheckOutDate = new DateTime(2025, 2, 25), GuestCount = 4, GuestName = "Jack Thomas", CreatedAt = new DateTime(2024, 12, 18, 9, 45, 0) },
                new Booking { BookingNumber = "BK013", RoomId = 4, CheckInDate = new DateTime(2025, 3, 1), CheckOutDate = new DateTime(2025, 3, 5), GuestCount = 1, GuestName = "Karen White", CreatedAt = new DateTime(2024, 12, 18, 11, 50, 0) },
                new Booking { BookingNumber = "BK014", RoomId = 17, CheckInDate = new DateTime(2025, 3, 10), CheckOutDate = new DateTime(2025, 3, 15), GuestCount = 2, GuestName = "Leo Harris", CreatedAt = new DateTime(2024, 12, 18, 13, 25, 0) },
                new Booking { BookingNumber = "BK015", RoomId = 30, CheckInDate = new DateTime(2025, 3, 20), CheckOutDate = new DateTime(2025, 3, 25), GuestCount = 4, GuestName = "Natalie Mason", CreatedAt = new DateTime(2024, 12, 18, 15, 10, 0) }
            });

            // Test-specific bookings for common test scenarios
            bookings.AddRange(new[]
            {
                new Booking { BookingNumber = "BK016", RoomId = 5, CheckInDate = new DateTime(2025, 1, 1), CheckOutDate = new DateTime(2025, 1, 5), GuestCount = 1, GuestName = "Christopher White Snr", CreatedAt = new DateTime(2024, 12, 18, 8, 30, 0) },
                new Booking { BookingNumber = "BK017", RoomId = 11, CheckInDate = new DateTime(2025, 1, 3), CheckOutDate = new DateTime(2025, 1, 7), GuestCount = 2, GuestName = "Oscar Rodriguez", CreatedAt = new DateTime(2024, 12, 18, 10, 15, 0) },
                new Booking { BookingNumber = "BK018", RoomId = 18, CheckInDate = new DateTime(2025, 1, 1), CheckOutDate = new DateTime(2025, 1, 5), GuestCount = 2, GuestName = "Trenton White", CreatedAt = new DateTime(2024, 12, 18, 12, 45, 0) },
                new Booking { BookingNumber = "BK019", RoomId = 35, CheckInDate = new DateTime(2025, 1, 3), CheckOutDate = new DateTime(2025, 1, 7), GuestCount = 3, GuestName = "Austin Whiten", CreatedAt = new DateTime(2024, 12, 18, 14, 55, 0) }
            });

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();
        }
    }
} 