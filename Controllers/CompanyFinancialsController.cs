using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CreditRiskAnalysisApp.Data;
using CreditRiskAnalysisApp.Models;

namespace CreditRiskAnalysisApp.Controllers
{
    public class CompanyFinancialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyFinancialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CompanyFinancials
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FinancialStatements.Include(c => c.FileName);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CompanyFinancials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyFinancial = await _context.FinancialStatements
                .Include(c => c.FileName)
                .FirstOrDefaultAsync(m => m.FileId == id);
            if (companyFinancial == null)
            {
                return NotFound();
            }

            return View(companyFinancial);
        }

        // GET: CompanyFinancials/Create
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            return View();
        }

        // POST: CompanyFinancials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (CompanyFinancial companyFinancial)
        {
            if (ModelState.IsValid)
            {

                _context.Add(companyFinancial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", companyFinancial.CompanyId);
            return View(companyFinancial);
        }

        // GET: CompanyFinancials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyFinancial = await _context.FinancialStatements.FindAsync(id);
            if (companyFinancial == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", companyFinancial.CompanyId);
            return View(companyFinancial);
        }

        // POST: CompanyFinancials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Revenue,Expenses,Profit,CompanyId")] CompanyFinancial companyFinancial)
        {
            if (id != companyFinancial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyFinancial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyFinancialExists(companyFinancial.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", companyFinancial.CompanyId);
            return View(companyFinancial);
        }

        // GET: CompanyFinancials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyFinancial = await _context.FinancialStatements
                .Include(c => c.FileName)
                .FirstOrDefaultAsync(m => m.FileId == id);
            if (companyFinancial == null)
            {
                return NotFound();
            }

            return View(companyFinancial);
        }

        // POST: CompanyFinancials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var companyFinancial = await _context.FinancialStatements.FindAsync(id);
            if (companyFinancial != null)
            {
                _context.FinancialStatements.Remove(companyFinancial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyFinancialExists(int id)
        {
            return _context.FinancialStatements.Any(e => e.FileId == id);
        }
    }
}
