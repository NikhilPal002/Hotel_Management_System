using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models
{
    public class IncomeDto
    {
        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "End date is required.")]
        public DateOnly EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public int NoOfBookings { get; set; }
    }

    public class IncomeReport
    {
        [Required(ErrorMessage = "Start date is required.")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "End date is required.")]
        public DateOnly EndDate { get; set; }
    }



}