using Microsoft.AspNetCore.Mvc;

namespace CreditRiskAnalysisApp.Controllers
{
    public class AnalysisFormController : Controller
    {
        public IActionResult InputForm()
        {
            ViewData["Title"] = "Analysis Form";
            return View();
        }
    }
}
