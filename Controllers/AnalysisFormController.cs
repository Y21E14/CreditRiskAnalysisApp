using Microsoft.AspNetCore.Mvc;
using CreditRiskAnalysisApp.Models;
using System.Text;
using System.Net.Http;
using System.Text.Json;

namespace CreditRiskAnalysisApp.Controllers
{
    public class AnalysisFormController : Controller
    {
        [HttpGet]
        public IActionResult InputForm()
        {
            ViewData["Title"] = "Predict Company Credit Risk";
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
                var payload = new
                {
                    Total_Asset = input.TotalAsset,
                    Cash = input.Cash,
                    Total_Debt_in_Current_Liabilities = input.TotalDebtInCurrentLiabilities,
                    Total_Long_Term_Debt = input.TotalLongTermDebt,
                    Earnings_Before_Interest = input.EarningsBeforeInterest,
                    Gross_Profit_Loss = input.GrossProfitLoss,
                    Total_Liabilities = input.TotalLiabilities,
                    Retained_Earnings = input.RetainedEarnings,
                    Total_Stockholders_Equity = input.TotalStockholdersEquity,
                    Total_Interest_and_Related_Expense = input.TotalInterestAndRelatedExpense,
                    Total_Market_Value = input.TotalMarketValue,
                    Total_Inventories = input.TotalInventories,
                    Total_Revenue = input.TotalRevenue,
                    Operating_Activities_Net_Cash_Flow = input.OperatingActivitiesNetCashFlow,
                    Financing_Activities_Net_Cash_Flow = input.FinancingActivitiesNetCashFlow,
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync("/predict", jsonContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var prediction = JsonSerializer.Deserialize<dynamic>(result);
                        ViewBag.CreditRiskResult = prediction["credit_risk_label"];
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

    }

}