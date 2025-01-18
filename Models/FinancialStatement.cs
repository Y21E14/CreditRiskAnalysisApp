using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CreditRiskAnalysisApp.Models
{
    public class FinancialStatement
    {
        [Key]
        // 'Id' is the primary key for the 'UploadFinancialStatement' table in the database
        public int FileId { get; set; }

        [Required]
        // store the name of the uploaded file
        public string FileName { get; set; } = string.Empty;

        [Required]
        // store the full path where the file is saved on the server
        public byte[] FileContent { get; set; } = Array.Empty<byte>();

        [Required]
        // captures the date and time when the file was uploaded
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        [ForeignKey("Company")]
        // stores the foreign key value linking this financial statement
        public int CompanyId { get; set; }

        // Defines a relationship to the 'Company' entity
        public Company Company { get; set; } = null!; // Navigation property
    }
}
