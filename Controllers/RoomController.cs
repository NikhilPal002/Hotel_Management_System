using AutoMapper;
using Hotel_Management.Data;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly IMapper mapper;

        public RoomController(HMDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            // Get list from domain model
            var roomDomain = await context.Rooms.ToListAsync();

            if (roomDomain == null || !roomDomain.Any())
            {
                return NotFound("No rooms available.");
            }

            // Map domain to DTO
            var roomDto = mapper.Map<List<RoomDto>>(roomDomain);
            // Return Dto
            return Ok(roomDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] AddUpdateRoomDto addRoomDto)
        {

            // Map Dto to Domain Model
            var roomDomain = mapper.Map<Room>(addRoomDto);

            // create room object
            await context.Rooms.AddAsync(roomDomain);
            await context.SaveChangesAsync();

            // Map domain model back to dto
            var roomDto = mapper.Map<RoomDto>(roomDomain);

            return Ok(roomDto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRoom([FromRoute] int id, [FromBody] AddUpdateRoomDto updateRoomDto)
        {

            // Map Dto to domain
            var roomDomain = mapper.Map<Room>(updateRoomDto);

            // check if record exist
            roomDomain = await context.Rooms.FirstOrDefaultAsync(x => x.RoomId == id);

            if (roomDomain == null)
            {
                return NotFound();
            }

            // update the fields
            roomDomain.RoomType = updateRoomDto.RoomType;
            roomDomain.Description = updateRoomDto.Description;
            roomDomain.NumberOfBeds = updateRoomDto.NumberOfBeds;
            roomDomain.PricePerNight = updateRoomDto.PricePerNight;
            roomDomain.Status = updateRoomDto.Status;

            await context.SaveChangesAsync();

            // Map domain model back to dto
            var roomDto = mapper.Map<RoomDto>(roomDomain);

            return Ok(roomDto);
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRoom(int id){
            var roomDomain = await context.Rooms.FirstOrDefaultAsync(x=> x.RoomId== id);

            if(roomDomain == null){
                return NotFound();
            }

            context.Rooms.Remove(roomDomain);
            await context.SaveChangesAsync();

            var roomDto = mapper.Map<RoomDto>(roomDomain);

            return Ok(roomDto);
        }
    }
}