using CreditRiskAnalysisApp.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CreditRiskAnalysisApp.Data // Ensure this matches your project's namespace
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Add DbSet properties for your models

        public DbSet<Company> Companies { get; set; }

        public DbSet<CompanyFinancial> CompanyFinancials { get; set; }

        public DbSet<UploadFinancialStatement> FinancialDocuments { get; set; }

        public DbSet<User> Users { get; set; }

    }
}
