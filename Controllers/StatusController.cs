using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CreditRiskAnalysisApp.Data;
using System.Linq;
using System.Text;

namespace CreditRiskAnalysisApp.Controllers
{
    public class StatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var companies = _context.Companies.ToList();

            // Pass dropdown options
            ViewBag.LoanStatusList = new SelectList(new[] { "Draft", "Approved", "Expired" });

            return View(companies);
        }

        [HttpPost]
        public IActionResult UpdateLoanStatus(int id, string loanStatus)
        {
            var company = _context.Companies.Find(id);
            if (company != null)
            {
                company.LoanStatus = loanStatus;
                _context.SaveChanges();

                // Set success message
                TempData["SuccessMessage"] = $"Loan status of {company.Name} is successfully updated as \"{loanStatus}\".";
            }
            return RedirectToAction("Index");
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Export Loan Status to CSV
        public IActionResult ExportLoanStatusToCSV()
        {
            var statuses = _context.Companies.Select(c => new
            {
                c.Name,
                c.UEN,
                c.LoanStatus
            }).ToList();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Company Name,UEN,Loan Status");

            foreach (var status in statuses)
            {
                csvBuilder.AppendLine($"{status.Name},{status.UEN},{status.LoanStatus}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(buffer, "text/csv", "LoanStatus.csv");
        }

    }
}
