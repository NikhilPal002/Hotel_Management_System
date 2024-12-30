using AutoMapper;
using Hotel_Management.Data;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly IMapper mapper;

        public BookingController(HMDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
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

        [HttpPost]
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

            var room = await context.Rooms.FirstOrDefaultAsync(r => r.RoomId == bookingDomain.RoomId);
            if (room == null)
            {
                return NotFound("Room not found.");
            }

            // If the room is already booked or it is available
            if (room.Status == "Booked" || room.Status == "Occupied")
            {
                return BadRequest("Room is already booked");
            }


            bookingDomain.TotalCost = room.PricePerNight * bookingDomain.NumberOfNights;
            bookingDomain.BookingStatus = "Pending";

            room.Status = "Booked";
            context.Rooms.Update(room);
            await context.Bookings.AddAsync(bookingDomain);
            await context.SaveChangesAsync();

            var bookingDto = mapper.Map<BookingDto>(bookingDomain);
            return Ok(bookingDto);
        }

        [HttpDelete]
        public async Task<IActionResult> Deletebooking(int id)
        {
            var bookingDomain = await context.Bookings.FirstOrDefaultAsync(x => x.BookingId == id);

            if (bookingDomain == null)
            {
                return NotFound("Booking not found");
            }

            context.Bookings.Remove(bookingDomain);
            await context.SaveChangesAsync();

            var bookingDto = mapper.Map<BookingDto>(bookingDomain);
            return Ok(bookingDto);
        }
    }
}