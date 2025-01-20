using AutoMapper;
using Hotel_Management.Services;
using Hotel_Management.Data;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Receptionist")]
    public class GuestController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly IMapper mapper;
        private readonly EmailService emailService;

        public GuestController(HMDbContext context, IMapper mapper, EmailService emailService)
        {
            this.context = context;
            this.mapper = mapper;
            this.emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var guest = await context.Guests.ToListAsync();
            return Ok(guest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGuestById([FromRoute] int id)
        {
            var guest = await context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);

            if (guest == null)
            {
                return NotFound("The guest is not found.");
            }

            var guestDto = mapper.Map<GuestDto>(guest);
            return Ok(guestDto);
        }


        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] AddUpdateGuestDto addGuestDto)
        {

            // Map Dto to Domain Model
            var guestDomain = mapper.Map<Guest>(addGuestDto);

            await context.Guests.AddAsync(guestDomain);
            await context.SaveChangesAsync();
            await emailService.SendEmailAsync
                (guestDomain.Email,
                "Welcome to hotel vistara",
                $"<h1>Welcome, {guestDomain.GuestName}!</h1>" +
                "<p>Your account has been successfully registered with Hotel Vistara.</p>" +
                "<p>If you did not register for this account, please contact our support team immediately.</p>" +
                "<p>Thank you for choosing Hotel vistara!</p>");

            // Map domain model back to dto
            var guestDto = mapper.Map<GuestDto>(guestDomain);

            return Ok(guestDto);
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateGuest([FromRoute] int id, [FromBody] AddUpdateGuestDto updateGuestDto)
        {

            if (updateGuestDto == null)
            {
                return BadRequest("Invalid guest data.");
            }

            // Map Dto to domain
            var guestDomain = mapper.Map<Guest>(updateGuestDto);

            guestDomain = await context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);

            if (guestDomain == null)
            {
                return NotFound("The guest is not found");
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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGuest([FromRoute] int id)
        {
            var guest = await context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);

            if (guest == null)
            {
                return NotFound("The guest is not found.");
            }

            context.Guests.Remove(guest);
            await context.SaveChangesAsync();

            return Ok(new { message = "Guest deleted successfully." });
        }

    }
}