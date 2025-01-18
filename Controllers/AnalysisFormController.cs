using Microsoft.AspNetCore.Mvc;
using CreditRiskAnalysisApp.Models;

namespace CreditRiskAnalysisApp.Controllers
{
    public class AnalysisFormController : Controller
    {
        [HttpGet]
        public IActionResult InputForm()
        {
            ViewData["Title"] = "Analysis Form";
            return View();
        }

        [HttpPost]
        public IActionResult InputForm(AnalysisInput input)
        {
            decimal totalCurrentLiabilities = input.TotalLongTermDebt - input.TotalDebtInCurrentLiabilities;

            var ratios = new
            {
                DebtToEquityRatio = (input.TotalDebtInCurrentLiabilities - totalCurrentLiabilities) / input.TotalStockholdersEquity,
                WorkingCapitalRatio = input.TotalAsset / totalCurrentLiabilities,
                DebtServiceCoverageRatio = input.EarningsBeforeInterest.HasValue ? input.EarningsBeforeInterest.Value / (input.TotalLongTermDebt + input.TotalDebtInCurrentLiabilities) : 0,
                GrossProfitMargin = (input.GrossProfitLoss / input.TotalRevenue) * 100
            };

            ViewBag.Ratios = ratios;
            return View();
        }
    }
}