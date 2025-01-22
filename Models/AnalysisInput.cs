namespace CreditRiskAnalysisApp.Models;
using System.ComponentModel.DataAnnotations;

    public class AnalysisInput
    {
        [Required(ErrorMessage = "Total Asset is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Asset must be a positive number")]
        public decimal TotalAsset { get; set; }

        [Required(ErrorMessage = "Cash is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cash must be a positive number")]
        public decimal Cash { get; set; }

        [Required(ErrorMessage = "Total Debt in Current Liabilities is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Debt in Current Liabilities must be a positive number")]
        public decimal TotalDebtInCurrentLiabilities { get; set; }

        [Required(ErrorMessage = "Total Long Term Debt is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Long Term Debt must be a positive number")]
        public decimal TotalLongTermDebt { get; set; }

        [Required(ErrorMessage = "Earnings before Interest is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Earnings before Interest must be a positive number")]
        public decimal? EarningsBeforeInterest { get; set; }
    
        public decimal GrossProfitLoss { get; set; }

        [Required(ErrorMessage = "Total Liabilities is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Liabilities must be a positive number")]
        public decimal TotalLiabilities { get; set; }

        public decimal RetainedEarnings { get; set; }

        [Required(ErrorMessage = "Total Stockholders Equity is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Stockholders Equity must be a positive number")]
        public decimal TotalStockholdersEquity { get; set; }

        public decimal TotalInterestAndRelatedExpense { get; set; }

        [Required(ErrorMessage = "Total Market Value is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Market Value must be a positive number")]
        public decimal? TotalMarketValue { get; set; }

        [Required(ErrorMessage = "Total Inventories is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Inventories must be a positive number")]
        public decimal TotalInventories { get; set; }

        [Required(ErrorMessage = "Total Revenue is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Revenue must be a positive number")]
        public decimal TotalRevenue { get; set; }

        public decimal OperatingActivitiesNetCashFlow { get; set; }

        public decimal FinancingActivitiesNetCashFlow { get; set; }
    }
    