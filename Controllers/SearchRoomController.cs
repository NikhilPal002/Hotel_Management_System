using AutoMapper;
using Hotel_Management.Data;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchRoomController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly IMapper mapper;

        public SearchRoomController(HMDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> SearchRoom([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            var roomQuery = context.Rooms.AsQueryable();

            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Type", StringComparison.OrdinalIgnoreCase))
                {
                    roomQuery = roomQuery.Where(x => x.RoomType.Contains(filterQuery));
                }
                else if (filterOn.Equals("NumberOfBeds", StringComparison.OrdinalIgnoreCase))
                {
                    // Try parsing the filterQuery to an integer
                    if (int.TryParse(filterQuery, out int numberOfBeds))
                    {
                        roomQuery = roomQuery.Where(x => x.NumberOfBeds == numberOfBeds);
                    }
                    else
                    {
                        return BadRequest($"Invalid value for NumberOfBeds: {filterQuery}. Please provide a valid integer.");
                    }
                }
                else
                {
                    return BadRequest($"Unsupported filter type: {filterOn}");
                }
            }

            // Execute the query and get results
            var roomDomain = await roomQuery.ToListAsync();
            if (!roomDomain.Any())
            {
                return NotFound("No rooms match the specified criteria.");
            }

            // // Map domain to DTO
            var roomDto = mapper.Map<List<RoomDto>>(roomDomain);
            // Return Dto
            return Ok(roomDto);
        }
    }
}