# Hotel Booking API

A RESTful API for hotel room booking built with ASP.NET Core and Entity Framework Core.

## Features

- **Hotel Management**: Search hotels by name.
- **Room Availability**: Find available rooms between dates for a given number of people.
- **Booking Management**: Create bookings and search by booking reference.
- **Data Management**: Seed and reset data for testing.
- **Business Rules**: 
  - Prevents double booking of rooms.
  - Enforces room capacity limits.
  - Ensures unique booking numbers.
  - Validates date ranges.

## Technology Stack

- ASP.NET Core 8.0
- Entity Framework Core 9.0
- SQL Server LocalDB
- Swagger/OpenAPI for documentation

## Database Schema

The API uses the following entities:

- **RoomType**: Single (1 guest), Double (2 guests), Deluxe (4 guests).
- **Hotel**: 7 hotels with addresses.
- **Room**: 6 rooms per hotel (2 Single, 2 Double, 2 Deluxe).
- **Booking**: Room bookings with guest information.

## API Endpoints

### Hotels

#### Get All Hotels
```http
GET /api/hotel
```

#### Search Hotels by Name
```http
GET /api/hotel/search?name={hotelName}
```

#### Get Hotel Details
```http
GET /api/hotel/{id}
```

### Rooms

#### Find Available Rooms
```http
GET /api/room/availability?checkInDate={date}&checkOutDate={date}&guestCount={count}&hotelId={id}
```

Parameters:
- `checkInDate`: Check-in date (YYYY-MM-DD).
- `checkOutDate`: Check-out date (YYYY-MM-DD).
- `guestCount`: Number of guests (minimum 1).
- `hotelId`: Optional hotel ID to filter by specific hotel.

#### Get Room Details
```http
GET /api/room/{id}
```

#### Get Rooms by Hotel
```http
GET /api/room/by-hotel/{hotelId}
```

### Bookings

#### Create Booking
```http
POST /api/booking
Content-Type: application/json

{
  "roomId": 1,
  "checkInDate": "2024-01-15",
  "checkOutDate": "2024-01-18",
  "guestCount": 2,
  "guestName": "John Doe"
}
```

#### Get Booking Details
```http
GET /api/booking/{bookingNumber}
```

#### Get Bookings by Room
```http
GET /api/booking/room/{roomId}
```

#### Delete Booking (Testing)
```http
DELETE /api/booking/{bookingNumber}
```

### Data Management (Testing)

#### Seed Database
```http
POST /api/data/seed
```

#### Reset Database
```http
POST /api/data/reset
```

#### Get Database Statistics
```http
GET /api/data/stats
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server LocalDB (installed with Visual Studio)

### Running the Application

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd WaracleTestAPI
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access the API**
   - API Base URL: `http://localhost:5244`
   - Swagger UI: `http://localhost:5244/swagger`

### Database Setup

The application will create the database and seed it with initial data on the first run. To reset the data:

1. **Reset all data**
   ```http
   POST http://localhost:5244/api/data/reset
   ```

2. **Seed with fresh data**
   ```http
   POST http://localhost:5244/api/data/seed
   ```

## Testing the API

### Example: Find Available Rooms

```bash
curl "http://localhost:5244/api/room/availability?checkInDate=2024-01-15&checkOutDate=2024-01-18&guestCount=2"
```

### Example: Create a Booking

```bash
curl -X POST "http://localhost:5244/api/booking" \
  -H "Content-Type: application/json" \
  -d '{
    "roomId": 1,
    "checkInDate": "2024-01-15",
    "checkOutDate": "2024-01-18",
    "guestCount": 2,
    "guestName": "John Doe"
  }'
```

### Example: Search Hotel

```bash
curl "http://localhost:5244/api/hotel/search?name=Azure"
```

## Business Rules Implementation

### Room Availability
- Rooms are available only if they have enough capacity for the guest count.
- No overlapping bookings are allowed for the same room.
- The check-in date must be before the check-out date.
- The check-in date cannot be in the past.

### Booking Creation
- Validates that the room can accommodate the guest count.
- Prevents double booking by checking for date overlaps.
- Generates unique booking numbers (format: BK{YYYYMMDD}{4-digit-random}).
- Enforces data validation on all required fields.

### Data Integrity
- Foreign key relationships exist between entities.
- Unique constraints apply to hotel names and booking numbers.
- Proper cascade delete behavior is implemented.

## Error Handling

The API returns appropriate HTTP status codes:

- `200 OK`: Successful requests.
- `201 Created`: Successful resource creation.
- `400 Bad Request`: Invalid request data.
- `404 Not Found`: Resource not found.
- `409 Conflict`: Business rule violations (like double booking).
- `500 Internal Server Error`: Server errors.

## Configuration

### Connection String

Update `appsettings.json` to change the database connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HotelBookingDB;Trusted_Connection=true;MultipleActiveResultSets=true;"
  }
}
```

### CORS

CORS is set to allow all origins for development. Update the CORS policy in `Program.cs` for production use.

## Architecture

### Project Structure
```
WaracleTestAPI/
├── Controllers/          # API controllers.
├── Data/                # DbContext and database configuration.
├── DTOs/                # Data transfer objects.
├── Models/              # Entity models.
├── Program.cs           # Application entry point.
└── appsettings.json     # Configuration.
```

### Design Patterns
- **Repository Pattern**: Entity Framework serves as the repository layer.
- **DTO Pattern**: Separate DTOs for API requests and responses.
- **Dependency Injection**: Services are registered in Program.cs.
- **RESTful Design**: Standard HTTP methods and status codes are used.

## Future Enhancements

Potential improvements for production use:

- Authentication and authorization.
- Rate limiting.
- Logging and monitoring.
- Input sanitization.
- Caching for performance.
- Background services for cleanup.
- Email notifications for bookings.
- Payment integration.
- Advanced search filters.