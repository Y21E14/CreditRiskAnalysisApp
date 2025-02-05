using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CreditRiskAnalysisApp.Data; // For ApplicationDbContext
using CreditRiskAnalysisApp.Models; // For the User model
using System.Text;
using System.Linq;
using X.PagedList;
using X.PagedList.Extensions;

namespace CreditRiskAnalysisApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page=1)
        {
            int pageSize = 10;
            var users = _context.Users.OrderBy(u => u.Name).ToPagedList(page ?? 1, pageSize);
            ViewBag.TotalUsers = users.TotalItemCount; // Store total count in ViewBag
            return View(users);
        }

        // GET: Create User
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user); // Display validation messages
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToAction(nameof(Index));
            
        }


        // GET: Edit User
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Edit User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Delete User
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Delete User
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "User deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult ExportUsersToCSV()
        {
            var users = _context.Users.ToList();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Name,NRIC,Email");

            foreach (var user in users)
            {
                csvBuilder.AppendLine($"{user.Name},{user.NRIC},{user.Email}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(buffer, "text/csv", "Users.csv");
        }
    }
}
