﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CreditRiskAnalysisApp.Data;
using CreditRiskAnalysisApp.Models;
using System.Text;

namespace CreditRiskAnalysisApp.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Company/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Companies.Add(company);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Company created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Check for model errors
            TempData["ErrorMessage"] = "Failed to create the company. Please check the details.";
            return View(company);
        }

        // GET: Company/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = _context.Companies.Find(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Companies.Any(e => e.Id == company.Id))
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
            return View(company);
        }
        // GET: Company/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = _context.Companies.FirstOrDefault(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var company = _context.Companies.Find(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: Company/Index
        public IActionResult Index()
        {
            var companies = _context.Companies.ToList();
            return View(companies);
        }

        // GET: Company/ViewFinancialStatements/{companyId}
        public IActionResult ViewFinancialStatements(int companyId)
        {
            var company = _context.Companies
                                  .Include(c => c.FinancialStatements)
                                  .FirstOrDefault(c => c.Id == companyId);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Upload company financial statement
        // GET: Company/Upload/{id}
        public IActionResult Upload(int id)
        {
            var company = _context.Companies.Find(id);
            if (company == null)
            {
                return NotFound();
            }

            ViewBag.CompanyId = id;
            ViewBag.CompanyName = company.Name;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int companyId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "No file selected. Please choose a file.";
                ViewBag.CompanyId = companyId;
                return View();
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                var financialStatement = new FinancialStatement
                {
                    CompanyId = companyId,
                    FileName = file.FileName,
                    FileContent = memoryStream.ToArray(),
                    UploadedAt = DateTime.Now
                };

                _context.FinancialStatements.Add(financialStatement);
                await _context.SaveChangesAsync();
            }

            TempData["Message"] = "File uploaded successfully!";
            return RedirectToAction("ViewFinancialStatements", new { companyId = companyId });
        }

        // GET: Company/EditFinancialStatement/{id}
        public IActionResult EditFinancialStatement(int id)
        {
            var statement = _context.FinancialStatements.Find(id);
            if (statement == null)
            {
                return NotFound();
            }

            ViewBag.CompanyName = _context.Companies.FirstOrDefault(c => c.Id == statement.CompanyId)?.Name;
            return View(statement);
        }

        // POST: Company/EditFinancialStatement/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFinancialStatement(int id, IFormFile newFile)
        {
            var statement = _context.FinancialStatements.Find(id);
            if (statement == null)
            {
                return NotFound();
            }

            if (newFile == null || newFile.Length == 0)
            {
                ViewBag.Message = "Please select a file to upload.";
                return View(statement);
            }

            using (var memoryStream = new MemoryStream())
            {
                await newFile.CopyToAsync(memoryStream);
                statement.FileName = newFile.FileName;
                statement.FileContent = memoryStream.ToArray();
                statement.UploadedAt = DateTime.Now;
            }

            _context.Update(statement);
            await _context.SaveChangesAsync();

            TempData["Message"] = "File updated successfully!";
            return RedirectToAction("ViewFinancialStatements", new { companyId = statement.CompanyId });
        }


        // GET: Company/DeleteFinancialStatement/{id}
        public IActionResult DeleteFinancialStatement(int id)
        {
            var statement = _context.FinancialStatements
                                    .Include(f => f.Company)
                                    .FirstOrDefault(f => f.FileId == id);

            if (statement == null)
            {
                return NotFound();
            }

            return View(statement);
        }


        // POST: Company/DeleteFinancialStatement/{id}
        [HttpPost, ActionName("DeleteFinancialStatement")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFinancialStatementConfirmed(int id)
        {
            var statement = _context.FinancialStatements.Find(id);
            if (statement != null)
            {
                int companyId = statement.CompanyId;
                _context.FinancialStatements.Remove(statement);
                _context.SaveChanges();

                TempData["Message"] = "Financial statement deleted successfully!";
                return RedirectToAction("ViewFinancialStatements", new { companyId = companyId });
            }

            return NotFound();
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public IActionResult ExportCompaniesToCSV()
        {
            var companies = _context.Companies.ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Company Name,UEN,Loan Status");

            foreach (var company in companies)
            {
                csv.AppendLine($"{company.Name},{company.UEN},{company.LoanStatus}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", "Companies_Report.csv");
        }
    }
}