using AutoMapper;
using Hotel_Management.Data;
using Hotel_Management.Models;
using Hotel_Management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Receptionist")]
    public class BookingController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly IMapper mapper;
        private readonly EmailService emailService;

        public BookingController(HMDbContext context, IMapper mapper,EmailService emailService)
        {
            this.context = context;
            this.mapper = mapper;
            this.emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Get list from domain model
            var bookingDomain = await context.Bookings.Include("Guest").Include("Room").ToListAsync();

            if (bookingDomain == null || !bookingDomain.Any())
            {
                return NotFound("No bookings available.");
            }

            // Map domain to DTO
            var bookingDto = mapper.Map<List<BookingDto>>(bookingDomain);
            // Return Dto
            return Ok(bookingDto);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] AddBookingDto addBookingDto)
        {

            // Map dto to domain
            var bookingDomain = mapper.Map<Booking>(addBookingDto);

            if (bookingDomain.CheckIn >= bookingDomain.CheckOut)
            {
                return BadRequest("Check-In date must be earlier than Check-Out date.");
            }

            if (bookingDomain.NumberOfAdults <= 0)
            {
                return BadRequest("Number of adults must be greater than 0.");
            }

            // Calculate the number of nights if not provided
            if (bookingDomain.NumberOfNights <= 0)
            {
                bookingDomain.NumberOfNights = (bookingDomain.CheckOut - bookingDomain.CheckIn).Days;
            }

            if (bookingDomain.NumberOfNights <= 0)
            {
                return BadRequest("The duration between Check-In and Check-Out must be positive.");
            }

            var guest = await context.Guests.FirstOrDefaultAsync(r => r.GuestId == bookingDomain.GuestId);
            if(guest == null){
                return NotFound("The guest profile is not found.");
            }

            var room = await context.Rooms.FirstOrDefaultAsync(r => r.RoomId == bookingDomain.RoomId);
            if (room == null)
            {
                return NotFound("Room not found.");
            }

            // If the room is already booked or it is available
            if (room.Status == "Occupied")
            {
                return BadRequest("Room is already occupied");
            }


            bookingDomain.TotalCost = room.PricePerNight * bookingDomain.NumberOfNights;
            bookingDomain.BookingStatus = "Booking Confirmed";
            bookingDomain.PaymentStatus = "Pending";

            room.Status = "Occupied";
            context.Rooms.Update(room);
            await context.Bookings.AddAsync(bookingDomain);
            await context.SaveChangesAsync();
             await emailService.SendEmailAsync
                (guest.Email,
                "Welcome to hotel vistara",
                $"<h1>Welcome, {guest.GuestName}!</h1>" +
                "<p>Your booking is successfully</p>" +
                $"<p>Your room no. is {room.RoomId}</p>" +
                $"<p>Your payment is {bookingDomain.PaymentStatus}</p>" +
                "<p>Thank you for choosing Hotel vistara!</p>");

            var bookingDto = mapper.Map<BookingDto>(bookingDomain);
            return Ok(bookingDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletebooking(int id)
        {
            var bookingDomain = await context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);

            if (bookingDomain == null)
            {
                return NotFound("Booking not found");
            }
            
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.RoomId == bookingDomain.RoomId);
            if(room != null){
                room.Status = "Available";
                context.Rooms.Update(room);
            }


            bookingDomain.BookingStatus = "Cancelled";
            bookingDomain.PaymentStatus = "Refunded";
            // context.Bookings.Remove(bookingDomain);
            await context.SaveChangesAsync();

            var bookingDto = mapper.Map<BookingDto>(bookingDomain);
            return Ok(bookingDto);
        }
    }
}