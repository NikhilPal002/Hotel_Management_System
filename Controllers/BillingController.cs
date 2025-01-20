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

        [HttpPost]
        public async Task<IActionResult> IssueBill([FromBody] AddBillingDto addBillingDto, [FromQuery] List<int> serviceIds)
        {
             // Validate input
            if (addBillingDto == null)
            {
                return BadRequest("Billing details cannot be null.");
            }
            if (serviceIds == null || !serviceIds.Any())
            {
                return BadRequest("Service IDs are required.");
            }

            var billingDomain = mapper.Map<Billing>(addBillingDto);

            // var booking = await context.Bookings.FindAsync(addBillingDto.BookingId);
            var booking = await context.Bookings
        .Include(b => b.Room) // Include Room details for price
        .FirstOrDefaultAsync(x => x.BookingId == addBillingDto.BookingId);

            if (booking == null)
            {
                return BadRequest("Booking not found.");
            }

            // Validate dates
            if (booking.CheckIn >= booking.CheckOut)
            {
                return BadRequest("CheckIn date must be earlier than CheckOut date.");
            }

            // Calculate the duration of stay
            var stayDuration = (booking.CheckOut - booking.CheckIn).Days;
            if (stayDuration <= 0)
            {
                return BadRequest("Invalid stay duration.");
            }

            // Fetch services from the database using the provided IDs
            var services = await context.Services
                .Where(s => serviceIds.Contains(s.Id))
                .ToListAsync();

            if (!services.Any())
            {
                return BadRequest("No valid services were selected.");
            }

            var datePart = DateTime.Now.ToString("yyyyMMdd");
            var uniquePart = Guid.NewGuid().ToString("N").Substring(0, 8);
            billingDomain.BillingNo = $"B-{datePart}-{uniquePart}";
            billingDomain.Price = booking.Room.PricePerNight;

            // Add the Billing record to the database
            await context.Billings.AddAsync(billingDomain);
            await context.SaveChangesAsync(); // Save to get the Billing ID            

            // Link the Billing with the selected Services
            foreach (var service in services)
            {
                var billingService = new BillingService
                {
                    BillingId = billingDomain.Id,
                    ServiceId = service.Id
                };
                await context.BillingServices.AddAsync(billingService);
            }

            // Calculate the total cost
            var serviceCost = services.Sum(s => s.ServiceCost);
            billingDomain.TotalCost = booking.TotalCost + billingDomain.Taxes + serviceCost;

            await context.SaveChangesAsync();


            // Generate the bill details
            var billDetails = $@"
        **** BILLING DETAILS ****
        Billing No: {billingDomain.BillingNo}
        Stay Dates: {booking.CheckIn:yyyy-MM-dd} to {booking.CheckOut:yyyy-MM-dd}
        Duration: {stayDuration} days
        Room Price: {billingDomain.Price:C} per day
        Services: {string.Join(", ", services.Select(s => s.ServiceName))}
        Service Cost: {serviceCost:C}
        Taxes: {billingDomain.Taxes:C}
        ----------------------------------
        Total Cost: {billingDomain.TotalCost:C}
        **********************************";

            // Return the bill details
            return Ok(billDetails);

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

    }


}