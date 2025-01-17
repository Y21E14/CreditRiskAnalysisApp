namespace CreditRiskAnalysisApp.Models
{
    public class AnalysisInput
    {
        public decimal TotalAsset { get; set; }
        public decimal Cash { get; set; }
        public decimal TotalDebtInCurrentLiabilities { get; set; }
        public decimal TotalLongTermDebt { get; set; }
        public decimal? EarningsBeforeInterest { get; set; }
        public decimal GrossProfitLoss { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal RetainedEarnings { get; set; }
        public decimal TotalStockholdersEquity { get; set; }
        public decimal TotalInterestAndRelatedExpense { get; set; }
        public decimal? TotalMarketValue { get; set; }
        public decimal TotalInventories { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal OperatingActivitiesNetCashFlow { get; set; }
        public decimal FinancingActivitiesNetCashFlow { get; set; }
    }
}