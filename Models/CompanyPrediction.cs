using CreditRiskAnalysisApp.Models;

public class CompanyPrediction
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }

    public string CreditRisk { get; set; }
    public int CreditRiskNumerical { get; set; }
    public decimal DebtServiceCoverageRatio { get; set; }
    public decimal DebtToEquityRatio { get; set; }
    public decimal GrossProfitMargin { get; set; }
    public decimal WorkingCapitalRatio { get; set; }

    public DateTime PredictionDate { get; set; }  // The date when the prediction was made
}
