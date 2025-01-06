using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CreditRiskAnalysisApp.Controllers
{
    public class FinancialDocumentsController : Controller
    {
        public IActionResult Upload()
        {
            ViewData["Title"] = "Upload Financial Documents";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Check if no file was uploaded or the file is empty
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "No file selected. Please choose a file.";
                return View();
            }

            // Define the upload path where files will be stored
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            // Ensure the upload directory exists
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath); // Creates the "upload" folder if it doesn't exists
            }

            // Combine the upload path with the file's name to get the full file path
            var filePath = Path.Combine(uploadPath, file.FileName);

            // Open a file stream to save the uploaded file to the server
            using (var stream = new FileStream(filePath, FileMode.Create)) // Opens or creates the file in "create" mode
            {
                await file.CopyToAsync(stream); // Asynchronously copies the file content from the upload to the file stream
            }

            ViewBag.Message = "File uploaded successfully!";
            return View();
        }
    }
}
