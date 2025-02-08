using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CreditRiskAnalysisApp.Data;
using X.PagedList;  
using System.Linq;
using System.Text;
using X.PagedList.Extensions;

namespace CreditRiskAnalysisApp.Controllers
{
    public class StatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page = 1)
        {
            int pageSize = 10;  // Show 5 records per page

            // Fetch companies and apply paging
            var companies = _context.Companies.OrderBy(c => c.Name).ToPagedList(page ?? 1, pageSize);

            // Pass dropdown options
            ViewBag.LoanStatusList = new List<string> { "Draft", "Approved", "Expired" };


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

                
                TempData["SuccessMessage"] = $"Loan status of {company.Name} is successfully updated as \"{loanStatus}\".";
            }
            return RedirectToAction("Index");
        }

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
