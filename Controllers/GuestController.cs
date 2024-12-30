using AutoMapper;
using Hotel_Management.Data;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly IMapper mapper;

        public GuestController(HMDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(){
            var guest = await context.Guests.ToListAsync();
            return Ok(guest);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddUpdateGuestDto addGuestDto){
            
            // Map Dto to Domain Model
            var guestDomain = mapper.Map<Guest>(addGuestDto);
            
            await context.Guests.AddAsync(guestDomain);
            await context.SaveChangesAsync();

            // Map domain model back to dto
            var guestDto = mapper.Map<GuestDto>(guestDomain);

            return Ok(guestDto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateGuest([FromRoute] int id ,[FromBody]AddUpdateGuestDto updateGuestDto){

            // Map Dto to domain
            var guestDomain = mapper.Map<Guest>(updateGuestDto);

            guestDomain = await context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);

            if(guestDomain == null){
                return NotFound();
            }

            guestDomain.GuestName = updateGuestDto.GuestName;
            guestDomain.Gender = updateGuestDto.Gender;
            guestDomain.Email = updateGuestDto.Email;
            guestDomain.PhoneNumber = updateGuestDto.PhoneNumber;
            guestDomain.State = updateGuestDto.State;
            guestDomain.PinCode = updateGuestDto.PinCode;

            await context.SaveChangesAsync();

            var guestDto = mapper.Map<GuestDto>(guestDomain);

            return Ok(guestDto);
        }
    }
}