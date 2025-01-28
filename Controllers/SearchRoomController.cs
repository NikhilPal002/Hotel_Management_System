using AutoMapper;
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
        public async Task<IActionResult> SearchRoom(
    [FromQuery] int? numberOfBeds,
    [FromQuery] DateOnly? checkInDate,
    [FromQuery] DateOnly? checkOutDate)
        {
            // Validate the date range
            if (checkInDate.HasValue && checkOutDate.HasValue && checkInDate >= checkOutDate)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid date range: check-in date must be before check-out date."
                });
            }

            var roomQuery = context.Rooms.AsQueryable();

            // Filter by number of beds if specified
            if (numberOfBeds.HasValue)
            {
                roomQuery = roomQuery.Where(x => x.NumberOfBeds == numberOfBeds);
            }

            // Filter for available rooms
            roomQuery = roomQuery.Where(x => x.Status == "Available");

            // Check availability for the given date range
            if (checkInDate.HasValue && checkOutDate.HasValue)
            {
                // Convert DateOnly to DateTime (set the time to midnight)
                DateTime checkInDateTime = checkInDate.Value.ToDateTime(TimeOnly.MinValue);
                DateTime checkOutDateTime = checkOutDate.Value.ToDateTime(TimeOnly.MinValue);

                roomQuery = roomQuery.Where(room =>
                    !context.Bookings.Any(booking =>
                        booking.RoomId == room.RoomId &&
                        (
                            (checkInDateTime >= booking.CheckIn && checkInDateTime < booking.CheckOut) || // Overlaps with check-in
                            (checkOutDateTime > booking.CheckIn && checkOutDateTime <= booking.CheckOut) || // Overlaps with check-out
                            (checkInDateTime <= booking.CheckIn && checkOutDateTime >= booking.CheckOut) // Encloses booking
                        )
                    ));
            }

            // Execute the query and get results
            var roomDomain = await roomQuery.ToListAsync();
            if (!roomDomain.Any())
            {
                return NotFound(new
                {
                    success = false,
                    message = "No rooms match the specified criteria."
                });
            }

            // Map domain to DTO
            var roomDto = mapper.Map<List<RoomDto>>(roomDomain);

            // Return structured JSON response
            return Ok(new
            {
                success = true,
                message = "Rooms fetched successfully.",
                data = roomDto
            });
        }

    }
}
