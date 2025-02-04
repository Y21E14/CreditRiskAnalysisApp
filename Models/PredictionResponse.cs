using System.Text.Json.Serialization;

namespace CreditRiskAnalysisApp.Models
{
    public class PredictionResponse
    {
        [JsonPropertyName("credit_risk_numerical")]
        public int CreditRiskNumerical { get; set; }

        [JsonPropertyName("credit_risk_label")]
        public string CreditRiskLabel { get; set; }

        [JsonPropertyName("calculated_ratios")]
        public CalculatedRatios CalculatedRatios { get; set; }
    }

    public class CalculatedRatios
    {
        [JsonPropertyName("Debt Service Coverage Ratio")]
        public decimal DebtServiceCoverageRatio { get; set; }

        [JsonPropertyName("Debt to Equity Ratio")]
        public decimal DebtToEquityRatio { get; set; }

        [JsonPropertyName("Gross Profit Margin")]
        public decimal GrossProfitMargin { get; set; }

        [JsonPropertyName("Working Capital Ratio")]
        public decimal WorkingCapitalRatio { get; set; }
    }
}
