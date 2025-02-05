using Microsoft.AspNetCore.Mvc;
using CreditRiskAnalysisApp.Data;
using CreditRiskAnalysisApp.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace CreditRiskAnalysisApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string riskCategory = "All", int? page = 1)
        {
            int pageSize = 5;  // Show 5 companies per page

            // Step 1: Load predictions and related companies
            var predictionsWithCompanies = _context.CompanyPredictions
                .Include(p => p.Company)
                .ToList();

            // Step 2: Group by CompanyId and select the most recent prediction
            var recentPredictions = predictionsWithCompanies
                .GroupBy(p => p.CompanyId)
                .Select(g => g.OrderByDescending(p => p.PredictionDate).FirstOrDefault())
                .ToList();

            // Sorting for Top 10 Best and Worst companies
            var topBestCompanies = recentPredictions
                .OrderBy(p => p.CreditRiskNumerical)  // Prefer lower risk numerically
                .ThenByDescending(p => p.GrossProfitMargin)  // Fall back on Gross Profit Margin
                .Take(10)  // Ensure we always have 10 companies
                .ToList();

            var topWorstCompanies = recentPredictions
                .OrderByDescending(p => p.CreditRiskNumerical)  // Sort by highest risk first
                .ThenBy(p => p.GrossProfitMargin)
                .Take(10)
                .ToList();

            // Filtering logic
            var filteredCompanies = riskCategory == "All"
                ? recentPredictions
                : recentPredictions.Where(c => c.CreditRisk == riskCategory).ToList();

            // Create a paged list for filtered companies
            var pagedCompanies = filteredCompanies.ToPagedList(page ?? 1, pageSize);

            // Populate the risk category filter dropdown
            ViewBag.RiskCategories = new List<SelectListItem>
            {
                new SelectListItem { Text = "All Companies", Value = "All", Selected = riskCategory == "All" },
                new SelectListItem { Text = "High Risk", Value = "High Risk", Selected = riskCategory == "High Risk" },
                new SelectListItem { Text = "Moderate Risk", Value = "Moderate Risk", Selected = riskCategory == "Moderate Risk" },
                new SelectListItem { Text = "Low Risk", Value = "Low Risk", Selected = riskCategory == "Low Risk" }
            };

            ViewBag.SelectedRisk = riskCategory;

            var model = new ReportViewModel
            {
                TopBestCompanies = topBestCompanies,
                TopWorstCompanies = topWorstCompanies,
                FilteredCompanies = pagedCompanies,  // Use the paged list here
                SelectedRisk = riskCategory
            };

            return View(model);
        }

        public async Task<IActionResult> ViewArchivedPredictions(int companyId)
        {
            var archivedPredictions = await _context.ArchivedPredictions
                .Include(p => p.Company)  // Include related Company details
                .Where(p => p.CompanyId == companyId)
                .OrderByDescending(p => p.PredictionDate)
                .ToListAsync();

            return View("ViewArchivedPredictions", archivedPredictions);
        }
    }
}
