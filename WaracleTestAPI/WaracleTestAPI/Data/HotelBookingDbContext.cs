using Microsoft.EntityFrameworkCore;
using WaracleTestAPI.Models;

namespace WaracleTestAPI.Data
{
    public class HotelBookingDbContext : DbContext
    {
        public HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure RoomType
            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.ToTable("RoomType");
                entity.HasKey(e => e.RoomTypeId);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Capacity).IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configure Hotel
            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.ToTable("Hotel");
                entity.HasKey(e => e.HotelId);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Address)
                    .HasMaxLength(255);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Configure Room
            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");
                entity.HasKey(e => e.RoomId);
                entity.Property(e => e.RoomNumber)
                    .IsRequired()
                    .HasMaxLength(10);
                
                // Foreign key relationships
                entity.HasOne(r => r.Hotel)
                    .WithMany(h => h.Rooms)
                    .HasForeignKey(r => r.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(r => r.RoomType)
                    .WithMany(rt => rt.Rooms)
                    .HasForeignKey(r => r.RoomTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Unique constraint on HotelId + RoomNumber
                entity.HasIndex(r => new { r.HotelId, r.RoomNumber }).IsUnique();
            });

            // Configure Booking
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");
                entity.HasKey(e => e.BookingId);
                
                entity.Property(e => e.BookingNumber)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.GuestName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.CheckInDate).IsRequired();
                entity.Property(e => e.CheckOutDate).IsRequired();
                entity.Property(e => e.GuestCount).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                
                // Foreign key relationship
                entity.HasOne(b => b.Room)
                    .WithMany(r => r.Bookings)
                    .HasForeignKey(b => b.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Unique constraint on BookingNumber
                entity.HasIndex(b => b.BookingNumber).IsUnique();
            });
            
            // Seed data
            // SeedData(modelBuilder); // Commented out to avoid conflicts with manual seeding
        }

        // Commented out to avoid conflicts with manual seeding via API
        /*
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed RoomTypes
            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { RoomTypeId = 1, Name = "Single", Capacity = 1 },
                new RoomType { RoomTypeId = 2, Name = "Double", Capacity = 2 },
                new RoomType { RoomTypeId = 3, Name = "Deluxe", Capacity = 4 }
            );

            // Seed Hotels
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel { HotelId = 1, Name = "Hotel Azure", Address = "1 Seaside Blvd" },
                new Hotel { HotelId = 2, Name = "Mountain Retreat", Address = "22 Hilltop Rd" },
                new Hotel { HotelId = 3, Name = "Urban Stay", Address = "100 Main St" },
                new Hotel { HotelId = 4, Name = "Grand Palace", Address = "9 Royal Ave" },
                new Hotel { HotelId = 5, Name = "Lakeside Inn", Address = "78 Lakeview Dr" },
                new Hotel { HotelId = 6, Name = "Skyview Hotel", Address = "200 Cloud St" },
                new Hotel { HotelId = 7, Name = "Sunset Villas", Address = "303 Sunset Blvd" }
            );

            // Seed Rooms (6 per hotel: 2 Single, 2 Double, 2 Deluxe)
            var rooms = new List<Room>();
            for (int hotelId = 1; hotelId <= 7; hotelId++)
            {
                // 2 Single rooms
                rooms.Add(new Room { RoomId = (hotelId - 1) * 6 + 1, HotelId = hotelId, RoomTypeId = 1, RoomNumber = $"{hotelId}01" });
                rooms.Add(new Room { RoomId = (hotelId - 1) * 6 + 2, HotelId = hotelId, RoomTypeId = 1, RoomNumber = $"{hotelId}02" });
                
                // 2 Double rooms
                rooms.Add(new Room { RoomId = (hotelId - 1) * 6 + 3, HotelId = hotelId, RoomTypeId = 2, RoomNumber = $"{hotelId}03" });
                rooms.Add(new Room { RoomId = (hotelId - 1) * 6 + 4, HotelId = hotelId, RoomTypeId = 2, RoomNumber = $"{hotelId}04" });
                
                // 2 Deluxe rooms
                rooms.Add(new Room { RoomId = (hotelId - 1) * 6 + 5, HotelId = hotelId, RoomTypeId = 3, RoomNumber = $"{hotelId}05" });
                rooms.Add(new Room { RoomId = (hotelId - 1) * 6 + 6, HotelId = hotelId, RoomTypeId = 3, RoomNumber = $"{hotelId}06" });
            }
            
            modelBuilder.Entity<Room>().HasData(rooms);
        }
        */
    }
} 