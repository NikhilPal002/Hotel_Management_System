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
            if (guest == null || !guest.Any())
            {
                return NotFound(new { message = "No guests found." });
            }

            return Ok(guest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGuestById([FromRoute] int id)
        {
            var guest = await context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);

            if (guest == null)
            {
                return NotFound(new { message = "The guest is not found." });
            }

            var guestDto = mapper.Map<GuestDto>(guest);
            return Ok(guestDto);
        }


        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] AddUpdateGuestDto addGuestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingGuest = await context.Guests
                .FirstOrDefaultAsync(g => g.Email.ToLower() == addGuestDto.Email.ToLower() || g.PhoneNumber == addGuestDto.PhoneNumber);

            if (existingGuest != null)
            {
                return BadRequest(new { message = "A guest with this email or phone number already exists." });
            }

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

            return CreatedAtAction(nameof(GetGuestById), new { id = guestDto.GuestId }, new
            {
                message = "Guest added successfully.",
                guestDto
            });
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateGuest([FromRoute] int id, [FromBody] AddUpdateGuestDto updateGuestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (updateGuestDto == null)
            {
                return BadRequest(new { message = "Invalid guest data." });
            }

            // Map Dto to domain
            var guestDomain = mapper.Map<Guest>(updateGuestDto);

            guestDomain = await context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);

            if (guestDomain == null)
            {
                return NotFound(new { message = "The guest is not found" });
            }

            // Check if new email or phone number already exists (excluding current guest)
            var duplicateGuest = await context.Guests
                .FirstOrDefaultAsync(g => (g.Email.ToLower() == updateGuestDto.Email.ToLower() || g.PhoneNumber == updateGuestDto.PhoneNumber) && g.GuestId != id);

            if (duplicateGuest != null)
            {
                return BadRequest(new { message = "A guest with this email or phone number already exists." });
            }

            guestDomain.GuestName = updateGuestDto.GuestName;
            guestDomain.Gender = updateGuestDto.Gender;
            guestDomain.Email = updateGuestDto.Email;
            guestDomain.PhoneNumber = updateGuestDto.PhoneNumber;
            guestDomain.State = updateGuestDto.State;
            guestDomain.PinCode = updateGuestDto.PinCode;

            await context.SaveChangesAsync();

            var guestDto = mapper.Map<GuestDto>(guestDomain);

            return Ok(new
            {
                message = "Guest Updated successfully.",
                guestDto
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGuest([FromRoute] int id)
        {
            var guest = await context.Guests.FirstOrDefaultAsync(x => x.GuestId == id);

            if (guest == null)
            {
                return NotFound(new { message = "The guest is not found" });
            }

            context.Guests.Remove(guest);
            await context.SaveChangesAsync();

            return Ok(new { message = "Guest deleted successfully." });
        }

    }
}