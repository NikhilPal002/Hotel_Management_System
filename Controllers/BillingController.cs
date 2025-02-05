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
            var billDomain = await context.Billings
                .Include(b => b.Booking)
                .ThenInclude(bk => bk.Guest)
                .Include(b => b.Booking)
            .ThenInclude(bk => bk.Room)
            .Include(b => b.BillingServices) // ✅ Include BillingServices
            .ThenInclude(bs => bs.Service)
                .Select(billing => new
                {
                    id = billing.Id,
                    billingNo = billing.BillingNo,
                    bookingId = billing.Booking != null ? billing.Booking.BookingId : (int?)null, // Check for null Booking
                    guestName = billing.Booking.Guest.GuestName != null ? billing.Booking.Guest.GuestName : "N/A", // Check for null Guest
                    roomCost = billing.Price,
                    roomNumber = billing.Booking.Room.RoomId,
                    taxes = billing.Taxes,
                    totalCost = billing.TotalCost,
                    services = billing.BillingServices.Select(bs => new
                    {
                        serviceName = bs.Service.ServiceName,
                        serviceCost = bs.Service.ServiceCost
                    }),
                    paymentStatus = billing.Booking.PaymentStatus ?? "Unknown"
                })
                .ToListAsync();

            if (billDomain == null || billDomain.Count == 0)
            {
                return NotFound(new { message = "No bills available." });
            }

            return Ok(billDomain);
        }


        [HttpPost("generate")]
        public async Task<IActionResult> GenerateBill([FromBody] AddBillingDto addBillingDto, [FromQuery] List<int> serviceIds)
        {
            if (addBillingDto == null || !serviceIds.Any())
                return BadRequest("Invalid input. Billing details and service IDs are required.");

            var billing = mapper.Map<Billing>(addBillingDto);

            // Fetch booking with room details
            var booking = await context.Bookings
                .Include(b => b.Room).Include(c => c.Guest)
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
            // var roomCost = stayDuration * booking.Room.PricePerNight;
            var serviceCost = services.Sum(s => s.ServiceCost);
            var totalCost = booking.TotalCost + serviceCost + billing.Taxes;

            billing.Price = booking.TotalCost;
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
                id = billing.Id,
                billingNo = billing.BillingNo,
                bookingId = booking.BookingId,
                guestName = billing.Booking.Guest.GuestName,
                roomCost = billing.Price,
                roomNumber = billing.Booking.Room.RoomId,
                ServiceCost = serviceCost,
                taxes = billing.Taxes,
                totalCost = billing.TotalCost,
                paymentStatus = booking.PaymentStatus,
                Services = services.Select(s => s.ServiceName),
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
            var billing = await context.Billings
                .Include(b => b.Booking) // Ensure Booking is loaded
                .ThenInclude(bk => bk.Guest)
                .Include(b => b.Booking) // ✅ Include Booking again to access Room
            .ThenInclude(bk => bk.Room)
                .Include(b => b.BillingServices)
                .ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(b => b.Id == billingId);

            if (billing == null)
            {
                return NotFound(new { message = "Billing record not found." });
            }

            if (billing.Booking == null)
            {
                return BadRequest(new { message = "Billing record exists, but Booking is missing." });
            }

            if (billing.Booking.Guest == null)
            {
                return BadRequest(new { message = "Billing record exists, but Guest information is missing." });
            }

             var totalServiceCost = billing.BillingServices.Sum(bs => bs.Service.ServiceCost);

            return Ok(new
            {
                id = billing.Id,
                billingNo = billing.BillingNo,
                bookingId = billing.Booking?.BookingId, // Ensure Booking exists before accessing BookingId
                guestName = billing.Booking?.Guest?.GuestName, // Ensure Guest exists before accessing GuestName
                roomCost = billing.Price,
                roomNumber = billing.Booking.Room.RoomId,
                taxes = billing.Taxes,
                serviceCost = totalServiceCost,
                totalCost = billing.TotalCost,

                services = billing.BillingServices.Select(bs => new
                {
                    serviceName = bs.Service.ServiceName,
                    serviceCost = bs.Service.ServiceCost
                }),
                paymentStatus = billing.Booking?.PaymentStatus // Ensure Booking exists before accessing PaymentStatus
            });
        }



    }
}
