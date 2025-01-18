using System.ComponentModel.DataAnnotations;

namespace CreditRiskAnalysisApp.Models
{
    public class Company
    {
        [Key] // Primary Key
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "UEN number is required.")]
        [StringLength(50)]
        public string UEN { get; set; } = string.Empty;

        // Loan Status: Draft, Approved, Expired
        [Required]
        [Display(Name = "Loan Status")]
        public string LoanStatus { get; set; } = "Draft";

        public ICollection<FinancialStatement> FinancialStatements { get; set; } = new List<FinancialStatement>();

    }
}

