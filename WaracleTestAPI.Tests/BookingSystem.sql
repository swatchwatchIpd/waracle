-- ========================================
-- Drop and Recreate Database
-- ========================================
USE master;
GO

IF DB_ID('HotelBookingDB') IS NOT NULL
BEGIN
    -- Force close all connections and set to single user mode
    ALTER DATABASE HotelBookingDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    
    -- Small delay to ensure connections are closed
    WAITFOR DELAY '00:00:02';
    
    DROP DATABASE HotelBookingDB;
END;

CREATE DATABASE HotelBookingDB;
GO

USE HotelBookingDB;
GO

-- ========================================
-- Drop Tables if They Exist (Safe Order)
-- ========================================
IF OBJECT_ID('Booking', 'U') IS NOT NULL DROP TABLE Booking;
IF OBJECT_ID('Room', 'U') IS NOT NULL DROP TABLE Room;
IF OBJECT_ID('Hotel', 'U') IS NOT NULL DROP TABLE Hotel;
IF OBJECT_ID('RoomType', 'U') IS NOT NULL DROP TABLE RoomType;
GO

-- ========================================
-- Create Tables
-- ========================================

-- 1. RoomType
CREATE TABLE RoomType (
    RoomTypeId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE CHECK (Name IN ('Single', 'Double', 'Deluxe')),
    Capacity INT NOT NULL CHECK (Capacity > 0)
);
GO

-- 2. Hotel
CREATE TABLE Hotel (
    HotelId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Address NVARCHAR(255)
);
GO

-- 3. Room
CREATE TABLE Room (
    RoomId INT PRIMARY KEY IDENTITY(1,1),
    HotelId INT NOT NULL,
    RoomTypeId INT NOT NULL,
    RoomNumber NVARCHAR(10) NOT NULL,
    CONSTRAINT FK_Room_Hotel FOREIGN KEY (HotelId) REFERENCES Hotel(HotelId),
    CONSTRAINT FK_Room_RoomType FOREIGN KEY (RoomTypeId) REFERENCES RoomType(RoomTypeId),
    CONSTRAINT UQ_Hotel_RoomNumber UNIQUE (HotelId, RoomNumber)
);
GO

-- 4. Booking
CREATE TABLE Booking (
    BookingId INT PRIMARY KEY IDENTITY(1,1),
    BookingNumber NVARCHAR(50) NOT NULL UNIQUE,
    RoomId INT NOT NULL,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    GuestCount INT NOT NULL CHECK (GuestCount > 0),
    GuestName NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Booking_Room FOREIGN KEY (RoomId) REFERENCES Room(RoomId),
    CONSTRAINT CHK_DateRange CHECK (CheckInDate < CheckOutDate)
);
GO

-- ========================================
-- Business Logic Validation
-- ========================================
-- NOTE: Business logic validation (double booking prevention, capacity checks, etc.) 
-- is now handled in the application layer (BookingService.cs) rather than database triggers.
-- This approach provides better error handling, testability, and compatibility with Entity Framework.

-- ========================================
-- Seed Data
-- ========================================

-- Insert Room Types
INSERT INTO RoomType (Name, Capacity)
VALUES 
    ('Single', 1),
    ('Double', 2),
    ('Deluxe', 4);
GO

-- Insert 7 Hotels
INSERT INTO Hotel (Name, Address)
VALUES 
    ('Hotel Azure', '1 Seaside Blvd'),
    ('Mountain Retreat', '22 Hilltop Rd'),
    ('Urban Stay', '100 Main St'),
    ('Grand Palace', '9 Royal Ave'),
    ('Lakeside Inn', '78 Lakeview Dr'),
    ('Skyview Hotel', '200 Cloud St'),
    ('Sunset Villas', '303 Sunset Blvd');
GO

-- Insert 6 Rooms per Hotel (2 Single, 2 Double, 2 Deluxe)
-- Room Layout by Hotel:
-- Hotel 1: Rooms 1-6   (1-2 Single, 3-4 Double, 5-6 Deluxe)
-- Hotel 2: Rooms 7-12  (7-8 Single, 9-10 Double, 11-12 Deluxe)
-- Hotel 3: Rooms 13-18 (13-14 Single, 15-16 Double, 17-18 Deluxe)
-- Hotel 4: Rooms 19-24 (19-20 Single, 21-22 Double, 23-24 Deluxe)
-- Hotel 5: Rooms 25-30 (25-26 Single, 27-28 Double, 29-30 Deluxe)
-- Hotel 6: Rooms 31-36 (31-32 Single, 33-34 Double, 35-36 Deluxe)
-- Hotel 7: Rooms 37-42 (37-38 Single, 39-40 Double, 41-42 Deluxe)

DECLARE @hotelId INT = 1;

WHILE @hotelId <= 7
BEGIN
    -- Room Numbers: e.g., 101, 102 for Single; 103, 104 for Double; 105, 106 for Deluxe

    -- 2 Single
    INSERT INTO Room (HotelId, RoomTypeId, RoomNumber) VALUES 
        (@hotelId, 1, CAST(@hotelId AS NVARCHAR) + '01'),
        (@hotelId, 1, CAST(@hotelId AS NVARCHAR) + '02');

    -- 2 Double
    INSERT INTO Room (HotelId, RoomTypeId, RoomNumber) VALUES 
        (@hotelId, 2, CAST(@hotelId AS NVARCHAR) + '03'),
        (@hotelId, 2, CAST(@hotelId AS NVARCHAR) + '04');

    -- 2 Deluxe
    INSERT INTO Room (HotelId, RoomTypeId, RoomNumber) VALUES 
        (@hotelId, 3, CAST(@hotelId AS NVARCHAR) + '05'),
        (@hotelId, 3, CAST(@hotelId AS NVARCHAR) + '06');

    SET @hotelId += 1;
END;
GO

-- ========================================
-- Seed Booking Data
-- ========================================

-- Insert sample bookings to support testing
-- This creates a mix of past, current, and future bookings across different hotels
-- Some rooms will be booked, others available for testing room availability

-- Past bookings (completed)
INSERT INTO Booking (BookingNumber, RoomId, CheckInDate, CheckOutDate, GuestCount, GuestName, CreatedAt)
VALUES 
    ('BK001', 1, '2024-01-15', '2024-01-18', 1, 'John Smith', '2024-01-10 10:00:00'),
    ('BK002', 8, '2024-01-20', '2024-01-25', 2, 'Jane Doe', '2024-01-15 14:30:00'),
    ('BK003', 18, '2024-02-01', '2024-02-05', 4, 'Bob Johnson', '2024-01-28 09:15:00');
GO

-- Current bookings (overlapping with common test dates)
INSERT INTO Booking (BookingNumber, RoomId, CheckInDate, CheckOutDate, GuestCount, GuestName, CreatedAt)
VALUES 
    ('BK004', 2, '2024-12-15', '2024-12-20', 1, 'Alice Brown', '2024-12-10 16:45:00'),
    ('BK005', 9, '2024-12-18', '2024-12-22', 2, 'Charlie Wilson', '2024-12-12 11:20:00'),
    ('BK006', 24, '2024-12-20', '2024-12-25', 3, 'Diana Miller', '2024-12-15 13:10:00');
GO 
-- Future bookings (for testing availability)
INSERT INTO Booking (BookingNumber, RoomId, CheckInDate, CheckOutDate, GuestCount, GuestName, CreatedAt)
VALUES 
    ('BK007', 3, '2025-01-10', '2025-01-15', 1, 'Eve Davis', '2024-12-18 10:30:00'),
    ('BK008', 16, '2025-01-15', '2025-01-20', 2, 'Frank Garcia', '2024-12-18 15:45:00'),
    ('BK009', 29, '2025-01-20', '2025-01-25', 4, 'Grace Martinez', '2024-12-18 12:15:00');
GO

-- Additional bookings across different hotels for comprehensive testing
INSERT INTO Booking (BookingNumber, RoomId, CheckInDate, CheckOutDate, GuestCount, GuestName, CreatedAt)
VALUES 
    ('BK010', 10, '2025-02-01', '2025-02-05', 2, 'Henry Anderson', '2024-12-18 14:20:00'),
    ('BK011', 12, '2025-02-10', '2025-02-15', 3, 'Iris Taylor', '2024-12-18 16:30:00'),
    ('BK012', 36, '2025-02-20', '2025-02-25', 4, 'Jack Thomas', '2024-12-18 09:45:00'),
    ('BK013', 4, '2025-03-01', '2025-03-05', 1, 'Karen White', '2024-12-18 11:50:00'),
    ('BK014', 17, '2025-03-10', '2025-03-15', 2, 'Leo Harris', '2024-12-18 13:25:00'),
    ('BK015', 30, '2025-03-20', '2025-03-25', 4, 'Mia Clark', '2024-12-18 15:10:00');
GO

-- Test-specific bookings for common test scenarios
-- These create specific availability scenarios for testing
INSERT INTO Booking (BookingNumber, RoomId, CheckInDate, CheckOutDate, GuestCount, GuestName, CreatedAt)
VALUES 
    -- Booking that blocks availability for Jan 1-5, 2025
    ('BK016', 5, '2025-01-01', '2025-01-05', 1, 'Nina Lopez', '2024-12-18 08:30:00'),
    -- Booking that blocks availability for Jan 3-7, 2025 (overlapping)
    ('BK017', 11, '2025-01-03', '2025-01-07', 2, 'Oscar Rodriguez', '2024-12-18 10:15:00'),
    -- Booking for a different hotel same dates to test hotel-specific availability
    ('BK018', 18, '2025-01-01', '2025-01-05', 2, 'Paula Martinez', '2024-12-18 12:45:00'),
    ('BK019', 35, '2025-01-03', '2025-01-07', 3, 'Quinn Johnson', '2024-12-18 14:55:00');
GO

-- ========================================
-- Application-Level Validation Notes
-- ========================================

-- The following business rules are now enforced in the BookingService.cs:
-- 1. Double booking prevention - checked via overlapping date queries
-- 2. Capacity validation - guest count verified against room type capacity
-- 3. Unique booking number generation - handled by GenerateUniqueBookingNumber method
-- 4. Date validation - check-in date must be before check-out date
-- 5. Past date validation - bookings cannot be made for past dates

-- Database-level constraints still enforced:
-- 1. Unique booking numbers (UNIQUE constraint)
-- 2. Valid date ranges (CHECK constraint: CheckInDate < CheckOutDate)
-- 3. Positive guest count (CHECK constraint: GuestCount > 0)
-- 4. Valid room types (CHECK constraint: Name IN ('Single', 'Double', 'Deluxe'))
-- 5. Foreign key relationships (FK constraints)

-- ========================================
-- Verification Queries (for debugging)
-- ========================================

-- Verify hotels and room counts
SELECT 
    h.Name,
    COUNT(r.RoomId) as RoomCount,
    COUNT(CASE WHEN rt.Name = 'Single' THEN 1 END) as SingleRooms,
    COUNT(CASE WHEN rt.Name = 'Double' THEN 1 END) as DoubleRooms,
    COUNT(CASE WHEN rt.Name = 'Deluxe' THEN 1 END) as DeluxeRooms
FROM Hotel h
LEFT JOIN Room r ON h.HotelId = r.HotelId
LEFT JOIN RoomType rt ON r.RoomTypeId = rt.RoomTypeId
GROUP BY h.HotelId, h.Name
ORDER BY h.Name;

-- Verify booking data
SELECT 
    b.BookingNumber,
    h.Name as HotelName,
    r.RoomNumber,
    rt.Name as RoomType,
    b.CheckInDate,
    b.CheckOutDate,
    b.GuestCount,
    b.GuestName
FROM Booking b
JOIN Room r ON b.RoomId = r.RoomId
JOIN Hotel h ON r.HotelId = h.HotelId
JOIN RoomType rt ON r.RoomTypeId = rt.RoomTypeId
ORDER BY b.CheckInDate, h.Name, r.RoomNumber;

-- Test: Find available rooms for 2 guests between Jan 1-5, 2025
-- This should show available rooms (some will be booked, others available)
SELECT 
    h.Name as HotelName,
    r.RoomNumber,
    rt.Name as RoomType,
    rt.Capacity
FROM Hotel h
JOIN Room r ON h.HotelId = r.HotelId
JOIN RoomType rt ON r.RoomTypeId = rt.RoomTypeId
WHERE rt.Capacity >= 2  -- For 2 guests
  AND NOT EXISTS (
    SELECT 1 FROM Booking b 
    WHERE b.RoomId = r.RoomId
      AND NOT (
        '2025-01-05' <= b.CheckInDate OR
        '2025-01-01' >= b.CheckOutDate
      )
  )
ORDER BY h.Name, r.RoomNumber;

-- Test: Find booking by reference
SELECT 
    b.BookingNumber,
    h.Name as HotelName,
    r.RoomNumber,
    rt.Name as RoomType,
    b.CheckInDate,
    b.CheckOutDate,
    b.GuestCount,
    b.GuestName,
    b.CreatedAt
FROM Booking b
JOIN Room r ON b.RoomId = r.RoomId
JOIN Hotel h ON r.HotelId = h.HotelId
JOIN RoomType rt ON r.RoomTypeId = rt.RoomTypeId
WHERE b.BookingNumber = 'BK010';

-- ========================================
-- Business Rules Validation Summary
-- ========================================

-- Rule 1: Hotels have 3 room types (single, double, deluxe)
SELECT 'Rule 1: Room Types' as ValidationTest;
SELECT rt.Name as RoomType, rt.Capacity, COUNT(*) as RoomCount
FROM RoomType rt
JOIN Room r ON rt.RoomTypeId = r.RoomTypeId
GROUP BY rt.Name, rt.Capacity
ORDER BY rt.Capacity;

-- Rule 2: Hotels have 6 rooms each
SELECT 'Rule 2: Rooms per Hotel' as ValidationTest;
SELECT h.Name as HotelName, COUNT(r.RoomId) as RoomCount
FROM Hotel h
LEFT JOIN Room r ON h.HotelId = r.HotelId
GROUP BY h.HotelId, h.Name
ORDER BY h.Name;

-- Rule 3: No double bookings
SELECT 'Rule 3: No Double Bookings' as ValidationTest;
SELECT 'Enforced by BookingService.CreateBookingAsync method' as Implementation;

-- Rule 4: No room changes during stay
SELECT 'Rule 4: No Room Changes' as ValidationTest;
SELECT 'Each booking record represents single room for entire stay' as Implementation;

-- Rule 5: Unique booking numbers
SELECT 'Rule 5: Unique Booking Numbers' as ValidationTest;
SELECT COUNT(DISTINCT BookingNumber) as UniqueBookings, COUNT(*) as TotalBookings
FROM Booking;

-- Rule 6: Guest count within room capacity
SELECT 'Rule 6: Capacity Compliance' as ValidationTest;
SELECT 
    b.BookingNumber,
    rt.Name as RoomType,
    rt.Capacity,
    b.GuestCount,
    CASE 
        WHEN b.GuestCount <= rt.Capacity THEN 'VALID'
        ELSE 'INVALID'
    END as ComplianceStatus
FROM Booking b
JOIN Room r ON b.RoomId = r.RoomId
JOIN RoomType rt ON r.RoomTypeId = rt.RoomTypeId
ORDER BY b.BookingNumber;

PRINT 'Database seeding completed successfully with business rules enforced in application layer!';
GO
