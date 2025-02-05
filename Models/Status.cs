using System.ComponentModel.DataAnnotations;

namespace CreditRiskAnalysisApp.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string CurrentStatus { get; set; } // Status 

        public string Description { get; set; }
    }
}
