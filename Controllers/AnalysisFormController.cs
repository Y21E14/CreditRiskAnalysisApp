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
            // Validate fields that cannot have negative values
            if (input.TotalAsset < 0)
            {
                ModelState.AddModelError(nameof(input.TotalAsset), "Total Asset must be a positive number.");
            }

            if (input.TotalLiabilities < 0)
            {
                ModelState.AddModelError(nameof(input.TotalLiabilities), "Total Liabilities must be a positive number.");
            }

            if (input.TotalStockholdersEquity <= 0)
            {
                ModelState.AddModelError(nameof(input.TotalStockholdersEquity), "Total Stockholders Equity must be greater than zero.");
            }

            if (input.TotalDebtInCurrentLiabilities < 0)
            {
                ModelState.AddModelError(nameof(input.TotalDebtInCurrentLiabilities), "Total Debt in Current Liabilities must be a positive number.");
            }

            if (input.TotalLongTermDebt < 0)
            {
                ModelState.AddModelError(nameof(input.TotalLongTermDebt), "Total Long-Term Debt must be a positive number.");
            }

            // If validation fails, return the view with errors
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            // Perform calculations
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

        [HttpPost]
        public IActionResult Submit(AnalysisInput input)
        {
            if (!ModelState.IsValid)
            {
                // Return the form with validation errors
                return View("InputForm", input);
            }
            // Proceed with processing valid input
            return RedirectToAction("Success");
        }

    }
}