using WaracleTestAPI.DTOs;
using WaracleTestAPI.Repositories;
using WaracleTestAPI.Utilities;

namespace WaracleTestAPI.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelRepository"></param>
        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<HotelSearchDto>> GetAllHotelsAsync()
        {
            var hotels = await _hotelRepository.GetAllHotelsAsync();
            return hotels.ToSearchDtos();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<HotelDto?> GetHotelByIdAsync(int id)
        {
            var hotel = await _hotelRepository.GetHotelByIdAsync(id);
            return hotel?.ToDto();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<IEnumerable<HotelSearchDto>> SearchHotelsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Enumerable.Empty<HotelSearchDto>();

            var hotels = await _hotelRepository.SearchHotelsByNameAsync(name);
            return hotels.ToSearchDtos();
        }
    }
} 