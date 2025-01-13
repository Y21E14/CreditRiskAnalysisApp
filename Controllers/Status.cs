using Microsoft.AspNetCore.Mvc;

namespace CreditRiskAnalysisApp.Controllers
{
    public class Status : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
