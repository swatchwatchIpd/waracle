using WaracleTestAPI.DTOs;
using WaracleTestAPI.Repositories;
using WaracleTestAPI.Utilities;

namespace WaracleTestAPI.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkInDate"></param>
        /// <param name="checkOutDate"></param>
        /// <param name="guestCount"></param>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<AvailableRoomDto>> GetAvailableRoomsAsync(DateTime checkInDate, DateTime checkOutDate, int guestCount, int? hotelId = null)
        {
            // Business logic validation
            if (checkInDate >= checkOutDate)
                throw new ArgumentException("Check-in date must be before check-out date");

            if (checkInDate.Date <= DateTime.Today)
                throw new ArgumentException("Check-in date cannot be in the past");

            if (guestCount < 1)
                throw new ArgumentException("Guest count must be at least 1");

            var rooms = await _roomRepository.GetAvailableRoomsAsync(checkInDate, checkOutDate, guestCount, hotelId);
            return rooms.ToAvailableRoomDtos();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RoomDto?> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            return room?.ToDto();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RoomDto>> GetRoomsByHotelIdAsync(int hotelId)
        {
            var rooms = await _roomRepository.GetRoomsByHotelIdAsync(hotelId);
            return rooms.ToRoomDtos();
        }
    }
} 