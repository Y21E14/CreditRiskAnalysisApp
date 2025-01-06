using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CreditRiskAnalysisApp.Models
{
    public class UploadFinancialStatement
    {
        [Key]
        // 'Id' is the primary key for the 'UploadFinancialStatement' table in the database
        public int Id { get; set; }

        [Required]
        // store the name of the uploaded file
        public string FileName { get; set; }

        [Required]
        // store the full path where the file is saved on the server
        public string FilePath { get; set; } 

        [Required]
        // stores the type or extension of the file (eg. '.pdf', '.docx')
        public string FileType { get; set; }

        // captures the date and time when the file was uploaded
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // stores the foreign key value linking this financial statement
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        // Defines a relationship to the 'Company' entity
        public Company Company { get; set; } // Navigation property
    }
}
