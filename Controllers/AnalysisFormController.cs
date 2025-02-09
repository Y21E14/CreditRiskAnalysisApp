using Microsoft.AspNetCore.Mvc;
using CreditRiskAnalysisApp.Models;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CreditRiskAnalysisApp.Data;

namespace CreditRiskAnalysisApp.Controllers
{

    public class AnalysisFormController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AnalysisFormController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]

        public IActionResult InputForm()
        {
            ViewData["Title"] = "Predict Company Credit Risk";

            var hasExistingPrediction = _context.CompanyPredictions.Any();
            ViewBag.HasExistingPrediction = hasExistingPrediction;


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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForm(AnalysisInput input)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000");

                // Construct the JSON payload
                var payload = new Dictionary<string, object>
{
                { "Total Asset", input.TotalAsset },
                { "Cash", input.Cash },
                { "Total Debt in Current Liabilities", input.TotalDebtInCurrentLiabilities },
                { "Total Long-Term Debt", input.TotalLongTermDebt },
                { "Earnings Before Interest", input.EarningsBeforeInterest },
                { "Gross Profit (Loss)", input.GrossProfitLoss }, 
                { "Total Liabilities", input.TotalLiabilities },
                { "Retained Earnings", input.RetainedEarnings },
                { "Total Stockholders Equity", input.TotalStockholdersEquity },
                { "Total Interest and Related Expense", input.TotalInterestAndRelatedExpense },
                { "Total Market Value", input.TotalMarketValue },
                { "Total Inventories", input.TotalInventories },
                { "Total Revenue", input.TotalRevenue },
                { "Operating Activities - Net Cash Flow", input.OperatingActivitiesNetCashFlow },
                { "Financing Activities - Net Cash Flow", input.FinancingActivitiesNetCashFlow }
};


                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null
                };
                var jsonContent = new StringContent(JsonSerializer.Serialize(payload, options), Encoding.UTF8, "application/json");


                try
                {
                    var response = await client.PostAsync("/predict", jsonContent);
                    var result = await response.Content.ReadAsStringAsync(); // Get raw JSON response

                    // For debugging purposes - log the response content to the console
                    Console.WriteLine("API Response: " + result);

                    if (response.IsSuccessStatusCode)
                    {
                        var prediction = JsonSerializer.Deserialize<PredictionResponse>(result);
                        if (prediction != null && prediction.CalculatedRatios != null)
                        {
                            ViewBag.Prediction = prediction;

                            // Load companies for the dropdown after displaying results
                            ViewBag.Companies = _context.Companies
                                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                                .ToList();
                        }
                        else
                        {
                            ViewBag.Error = $"Failed to get a valid prediction response. Response Content: {result}";
                        }
                    }
                    else
                    {
                        ViewBag.Error = $"Error: {response.StatusCode} - {response.ReasonPhrase}";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"An exception occurred: {ex.Message}";
                }

            }

            return View("InputForm", input);
        }

        [HttpPost]
        public async Task<IActionResult> SavePrediction(int companyId, PredictionResponse prediction)
        {
            // Find the most recent prediction for the company
            var existingPrediction = await _context.CompanyPredictions
                .Where(p => p.CompanyId == companyId)
                .OrderByDescending(p => p.PredictionDate)
                .FirstOrDefaultAsync();

            // Archive the old prediction if it exists
            if (existingPrediction != null)
            {
                var archivedPrediction = new ArchivedPrediction
                {
                    CompanyId = existingPrediction.CompanyId,
                    CreditRisk = existingPrediction.CreditRisk,
                    CreditRiskNumerical = existingPrediction.CreditRiskNumerical,
                    DebtServiceCoverageRatio = existingPrediction.DebtServiceCoverageRatio,
                    DebtToEquityRatio = existingPrediction.DebtToEquityRatio,
                    GrossProfitMargin = existingPrediction.GrossProfitMargin,
                    WorkingCapitalRatio = existingPrediction.WorkingCapitalRatio,
                    PredictionDate = existingPrediction.PredictionDate
                };

                _context.ArchivedPredictions.Add(archivedPrediction);
                _context.CompanyPredictions.Remove(existingPrediction);
            }

            // To create and save the new prediction
            var newPrediction = new CompanyPrediction
            {
                CompanyId = companyId,
                CreditRisk = prediction.CreditRiskLabel,
                CreditRiskNumerical = prediction.CreditRiskNumerical,
                DebtServiceCoverageRatio = prediction.CalculatedRatios.DebtServiceCoverageRatio,
                DebtToEquityRatio = prediction.CalculatedRatios.DebtToEquityRatio,
                GrossProfitMargin = prediction.CalculatedRatios.GrossProfitMargin,
                WorkingCapitalRatio = prediction.CalculatedRatios.WorkingCapitalRatio,
                PredictionDate = DateTime.Now
            };

            _context.CompanyPredictions.Add(newPrediction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Report");
        }


    }

}
