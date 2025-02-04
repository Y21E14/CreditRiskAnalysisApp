using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreditRiskAnalysisApp.Models
{
    public class ArchivedPrediction
    {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public string CreditRisk { get; set; }

        public int CreditRiskNumerical { get; set; }

        public decimal DebtServiceCoverageRatio { get; set; }

        public decimal DebtToEquityRatio { get; set; }

        public decimal GrossProfitMargin { get; set; }

        public decimal WorkingCapitalRatio { get; set; }

        public DateTime PredictionDate { get; set; }
    }
}
