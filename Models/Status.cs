using System.ComponentModel.DataAnnotations;

namespace CreditRiskAnalysisApp.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; } // Unique identifier

        [Required]
        public string CurrentStatus { get; set; } // Status 

        public string Description { get; set; } // Optional: Details about the status
    }
}
