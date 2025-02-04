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

        public DbSet<FinancialStatement> FinancialStatements { get; set; }

        public DbSet<AnalysisInput> AnalysisInputs { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Status> Statuses { get; set; }

        public DbSet<CompanyPrediction> CompanyPredictions { get; set; }

        public DbSet<ArchivedPrediction> ArchivedPredictions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Specify precision for decimal fields in CompanyPrediction
            modelBuilder.Entity<CompanyPrediction>().Property(p => p.WorkingCapitalRatio).HasPrecision(18, 2);
            modelBuilder.Entity<CompanyPrediction>().Property(p => p.DebtToEquityRatio).HasPrecision(18, 2);
            modelBuilder.Entity<CompanyPrediction>().Property(p => p.GrossProfitMargin).HasPrecision(18, 2);
            modelBuilder.Entity<CompanyPrediction>().Property(p => p.DebtServiceCoverageRatio).HasPrecision(18, 2);

            // Specify precision for decimal fields in ArchivedPrediction
            modelBuilder.Entity<ArchivedPrediction>().Property(p => p.WorkingCapitalRatio).HasPrecision(18, 2);
            modelBuilder.Entity<ArchivedPrediction>().Property(p => p.DebtToEquityRatio).HasPrecision(18, 2);
            modelBuilder.Entity<ArchivedPrediction>().Property(p => p.GrossProfitMargin).HasPrecision(18, 2);
            modelBuilder.Entity<ArchivedPrediction>().Property(p => p.DebtServiceCoverageRatio).HasPrecision(18, 2);

            // Specify precision for fields in AnalysisInput (optional based on warnings)
            modelBuilder.Entity<AnalysisInput>().Property(a => a.Cash).HasPrecision(18, 2);
            modelBuilder.Entity<AnalysisInput>().Property(a => a.TotalAsset).HasPrecision(18, 2);
            modelBuilder.Entity<AnalysisInput>().Property(a => a.TotalLiabilities).HasPrecision(18, 2);
            modelBuilder.Entity<AnalysisInput>().Property(a => a.GrossProfitLoss).HasPrecision(18, 2);
        }
    }
}

