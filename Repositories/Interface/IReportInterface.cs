using Hotel_Management.Models;

namespace Hotel_Management.Repositories
{
    public interface IReportInterface
    {
        Task<List<StaffPaymentDto>> GetStaffPaymentAsync(DateTime startDate, DateTime endDate);

        Task<IncomeDto> GetIncomeReportAsync(IncomeReport report);
    }
}