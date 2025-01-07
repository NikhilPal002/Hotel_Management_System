using AutoMapper;
using Hotel_Management.Services;
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
        private readonly EmailService emailService;

        public PaymentController(HMDbContext context, IMapper mapper, EmailService emailService)
        {
            this.context = context;
            this.mapper = mapper;
            this.emailService = emailService;
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

            var bill = await context.Billings.Include(b => b.Booking).ThenInclude(b => b.Guest).FirstOrDefaultAsync(x => x.Id == paymentDomain.BillingId);
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

            if (bill.Booking.Room != null) // Ensure Room is not null
            {
                bill.Booking.Room.Status = "Available";
            }

            await context.Payments.AddAsync(paymentDomain);
            await context.SaveChangesAsync();
            await emailService.SendEmailAsync
                (paymentDomain.Billing.Booking.Guest.Email,
                "<h6 style='color: #0044cc;'>📅 Payment Successfully Received!</h6>",
                $@"<p>Dear {paymentDomain.Billing.Booking.Guest.GuestName},</p>
                <p>Your recent payment of ₹{paymentDomain.PaymentAmount} has been successfully processed!</p>");

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