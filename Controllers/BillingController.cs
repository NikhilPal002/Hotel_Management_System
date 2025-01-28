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
    // [Authorize(Roles = "Receptionist")]
    public class BillingController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly IMapper mapper;

        public BillingController(HMDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            // Get list from domain model
            var billDomain = await context.Billings.Include("Booking").ToListAsync();

            if (billDomain == null || !billDomain.Any())
            {
                return NotFound("No rooms available.");
            }

            // Map domain to DTO
            var billingDto = mapper.Map<List<BillingDto>>(billDomain);
            // Return Dto
            return Ok(billingDto);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateBill([FromBody] AddBillingDto addBillingDto, [FromQuery] List<int> serviceIds)
        {
            if (addBillingDto == null || !serviceIds.Any())
                return BadRequest("Invalid input. Billing details and service IDs are required.");

            var billing = mapper.Map<Billing>(addBillingDto);

            // Fetch booking with room details
            var booking = await context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(x => x.BookingId == addBillingDto.BookingId);

            if (booking == null)
                return BadRequest("Booking not found.");

            // Validate stay duration
            var stayDuration = (booking.CheckOut - booking.CheckIn).Days;
            if (stayDuration <= 0)
                return BadRequest("Invalid stay duration.");

            // Fetch and validate services
            var services = await context.Services
                .Where(s => serviceIds.Contains(s.Id))
                .ToListAsync();
            if (!services.Any())
                return BadRequest("No valid services were selected.");

            // Generate unique billing number
            billing.BillingNo = $"B-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 8);

            // Calculate costs
            var roomCost = stayDuration * booking.Room.PricePerNight;
            var serviceCost = services.Sum(s => s.ServiceCost);
            var totalCost = roomCost + serviceCost + billing.Taxes;

            billing.Price = booking.Room.PricePerNight;
            billing.TotalCost = totalCost;

            // Save billing record
            await context.Billings.AddAsync(billing);
            await context.SaveChangesAsync();

            // Link services with the bill
            var billingServices = services.Select(s => new BillingService
            {
                BillingId = billing.Id,
                ServiceId = s.Id
            });
            await context.BillingServices.AddRangeAsync(billingServices);
            await context.SaveChangesAsync();

            return Ok(new
            {
                BillingNo = billing.BillingNo,
                RoomCost = roomCost,
                ServiceCost = serviceCost,
                Taxes = billing.Taxes,
                TotalCost = totalCost,
                Services = services.Select(s => s.ServiceName)
            });

        }

        [HttpGet("services")]
        public async Task<IActionResult> GetServices()
        {
            var services = await context.Services.AsNoTracking().ToListAsync();

            if (!services.Any())
            {
                return Ok(new List<Service>()); // Return empty array instead of 404
            }

            return Ok(services);
        }

        [HttpGet("{billingId}")]
        public async Task<IActionResult> GetBillingDetails(int billingId)
        {
            var billing = await context.Billings.Include(b => b.BillingServices)
                .ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(b => b.Id == billingId);

            if (billing == null)
                return NotFound("Billing record not found.");

            return Ok(billing);
        }


    }
}
