using Hotel_Management.Data;
using Hotel_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Repositories
{
    public class ReportRepository : IReportInterface
    {
        private readonly HMDbContext context;

        public ReportRepository(HMDbContext context)
        {
            this.context = context;
        }

        public async Task<IncomeDto> GetIncomeReportAsync(IncomeReport report)
        {
            var income = await context.Payments.Include(b => b.Billing)
            .ThenInclude(bo => bo.Booking)
                .Where(p => p.Billing.Booking.PaymentStatus == "Paid" &&
                            p.PaymentDate >= report.StartDate.ToDateTime(TimeOnly.MinValue) &&
                            p.PaymentDate <= report.EndDate.ToDateTime(TimeOnly.MaxValue))
                .GroupBy(p => true)
                .Select(g => new IncomeDto
                {
                    StartDate = report.StartDate,
                    EndDate = report.EndDate,
                    TotalIncome = g.Sum(p => p.PaymentAmount),
                    NoOfBookings = g.Count()
                })
                .FirstOrDefaultAsync();

            return income ?? new IncomeDto { StartDate = report.StartDate, EndDate = report.EndDate, TotalIncome = 0, NoOfBookings = 0 };
        }


        public async Task<List<StaffPaymentDto>> GetStaffPaymentAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate.Day != 1 || endDate.Day != 1)
            {
                throw new ArgumentException("Start and End dates must be the first day of their respective months.");
            }

            if (startDate >= endDate)
            {
                throw new ArgumentException("StartDate must be before EndDate.");
            }

            var staffList = await context.Staffs.ToListAsync();
            List<StaffPaymentDto> staffPaymentList = new List<StaffPaymentDto>();

            foreach (var staff in staffList)
            {
                if (staff.JoinDate > endDate)
                {
                    continue;
                }

                DateTime adjustedStartDate = staff.JoinDate;

                if (staff.JoinDate.Day > 15)
                {
                    adjustedStartDate = new DateTime(staff.JoinDate.Year, staff.JoinDate.Month, 1).AddMonths(1);
                }

                if (adjustedStartDate < startDate || adjustedStartDate > endDate)
                {
                    continue;
                }

                decimal totalPayment = CalculatePayment(staff.Salary, staff.JoinDate, startDate, endDate);

                staffPaymentList.Add(new StaffPaymentDto
                {
                    StaffId = staff.StaffId,
                    StaffName = staff.FullName,
                    TotalPayment = totalPayment
                });
            }

            return staffPaymentList;
        }

        private decimal CalculatePayment(decimal salary, DateTime hireDate, DateTime startDate, DateTime endDate)
        {
            if (hireDate > startDate)
            {
                startDate = hireDate;
            }

            endDate = endDate.AddDays(-1);

            if (startDate > endDate)
            {
                return 0;
            }

            decimal totalPayment = 0;
            DateTime currentDate = startDate;

            while (currentDate <= endDate)
            {
                int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                decimal dailyRate = salary / daysInMonth;

                int startDay = currentDate.Day;
                int endDay = (currentDate.Month == endDate.Month && currentDate.Year == endDate.Year)
                    ? endDate.Day
                    : daysInMonth;

                totalPayment += dailyRate * (endDay - startDay + 1);

                currentDate = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(1);
            }

            return Math.Round(totalPayment, 4);
        }
    }
}