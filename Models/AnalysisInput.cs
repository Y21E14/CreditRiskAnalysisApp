namespace CreditRiskAnalysisApp.Models
{
    public class AnalysisInput
    {
        public string? CompanyName { get; set; }
        public int CreditScore { get; set; }

        public int Id { get; set; } // Primary Key
        public string InputName { get; set; }
        public string Description { get; set; }
    }
}