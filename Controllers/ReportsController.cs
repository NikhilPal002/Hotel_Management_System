using Hotel_Management.Models;
using Hotel_Management.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize(Roles = "Owner")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportInterface reportInterface;

        public ReportsController(IReportInterface reportInterface)
        {
            this.reportInterface = reportInterface;
        }

        [HttpGet("StaffPaymentReport")]
        public async Task<IActionResult> GetStaffPayment(DateTime startDate, DateTime endDate)
        {

            try
            {
                var staffPayments = await reportInterface.GetStaffPaymentAsync(startDate, endDate);
                decimal totalPayments = staffPayments.Sum(sp => sp.TotalPayment);
                int totalStaff = staffPayments.Count;

                return Ok(new { startDate, endDate, totalPayments, totalStaff });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("IncomeReport")]
        public async Task<IActionResult> GetIncomeReport([FromBody] IncomeReport report)
        {
            if (report == null || report.StartDate == default || report.EndDate == default)
            {
                return BadRequest(new { message = "Invalid request. StartDate and EndDate are required." });
            }

            if (report.StartDate >= report.EndDate)
            {
                return BadRequest(new { message = "StartDate must be before EndDate." });
            }

            var income = await reportInterface.GetIncomeReportAsync(report);

            return Ok(income);
        }


    }
}