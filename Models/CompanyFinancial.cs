using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CreditRiskAnalysisApp.Models
{
    public class CompanyFinancial
    {
        [Key] // Primary Key
        public int Id { get; set; }

        [Required(ErrorMessage = "Revenue is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Revenue must be a positive number.")]
        public double Revenue { get; set; }

        [Required(ErrorMessage = "Expenses are required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Expenses must be a positive number.")]
        public double Expenses { get; set; }

        [Required(ErrorMessage = "Profit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Profit must be a positive number.")]
        public double Profit { get; set; }

        [Required]
        [ForeignKey("Company")] // Foreign key reference
        public int CompanyId { get; set; }
        public Company company { get; set; }
    }
}
