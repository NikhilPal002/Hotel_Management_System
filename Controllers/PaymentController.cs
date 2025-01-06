using AutoMapper;
using Hotel_Management.Data;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly HMDbContext context;
        private readonly IMapper mapper;

        public PaymentController(HMDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] AddPaymentDto addPaymentDto)
        {
            if (addPaymentDto == null)
            {
                return BadRequest("Payment data is missing.");
            }

            var paymentDomain = mapper.Map<Payment>(addPaymentDto);

            if (paymentDomain == null || paymentDomain.BillingId == null)
            {
                return BadRequest("Invalid payment data.");
            }

            var bill = await context.Billings.Include(b=>b.Booking).FirstOrDefaultAsync(x => x.Id == paymentDomain.BillingId);
            if (bill == null)
            {
                return BadRequest("Billing details invalid");
            }

            if (bill.Booking == null)
            {
                return BadRequest("Booking Not Found");
            }

            if (bill.Booking.PaymentStatus == "Paid")
            {
                return BadRequest("Payment has already been completed for this booking.");
            }

            paymentDomain.PaymentAmount = bill.TotalCost;
            // Update booking and save payment
            bill.Booking.PaymentStatus = "Paid";
            paymentDomain.PaymentDate = DateTime.Now;

            // Inline logic to generate a unique Transaction ID
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var randomSuffix = new Random().Next(1000, 9999); // Generates a random 4-digit number

            paymentDomain.TransactionId = $"TXN-{timestamp}-{randomSuffix}";

            await context.Payments.AddAsync(paymentDomain);
            await context.SaveChangesAsync();

            var paymentDto = mapper.Map<PaymentDto>(paymentDomain);

            return Ok(new
            {
                message = "Payment processed successfully!",
                paymentDto
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPayment()
        {

            var paymentDomain = await context.Payments.Include("Booking").ToListAsync();

            if (paymentDomain == null)
            {
                return BadRequest("No payment available");
            }

            var paymentDto = mapper.Map<List<PaymentDto>>(paymentDomain);

            return Ok(paymentDto);
        }
    }
}