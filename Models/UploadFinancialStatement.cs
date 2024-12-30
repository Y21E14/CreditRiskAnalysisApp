using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CreditRiskAnalysisApp.Models
{
    public class UploadFinancialStatement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } // Name of the file

        [Required]
        public string FilePath { get; set; } // Path where the file is stored

        [Required]
        public string FileType { get; set; } // File type (e.g., .pdf, .docx)

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; } // Navigation property
    }
}
