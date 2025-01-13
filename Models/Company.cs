using System.ComponentModel.DataAnnotations;

namespace CreditRiskAnalysisApp.Models
{
    public class Company
    {
        [Key] // Primary Key
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Registration number is required.")]
        [StringLength(50)]
        public string RegistrationNumber { get; set; }

        [StringLength(100, ErrorMessage = "Sector cannot exceed 100 characters.")]
        public string Sector { get; set; }

        // Navigation property for related financial data
        public ICollection<CompanyFinancial> Financials { get; set; } = 
            new List<CompanyFinancial>();
    }
}

