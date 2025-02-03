using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditRiskAnalysisApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateCompanyPredictionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //name: "FK_CompanyPredictions_FinancialStatements_FinancialStatementId",
            //table: "CompanyPredictions");

            migrationBuilder.Sql(
    @"IF EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_CompanyPredictions_FinancialStatementId') 
      BEGIN 
          DROP INDEX IX_CompanyPredictions_FinancialStatementId ON CompanyPredictions 
      END");


            migrationBuilder.Sql(
      @"IF EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS 
                 WHERE TABLE_NAME = 'CompanyPredictions' 
                 AND COLUMN_NAME = 'FinancialStatementId')
      BEGIN 
          ALTER TABLE CompanyPredictions 
          DROP COLUMN FinancialStatementId 
      END");

            migrationBuilder.Sql(
    @"IF EXISTS (SELECT 1 
                 FROM INFORMATION_SCHEMA.COLUMNS 
                 WHERE TABLE_NAME = 'CompanyPredictions' 
                 AND COLUMN_NAME = 'IsCurrent')
      BEGIN 
          ALTER TABLE CompanyPredictions 
          DROP COLUMN IsCurrent 
      END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinancialStatementId",
                table: "CompanyPredictions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "CompanyPredictions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPredictions_FinancialStatementId",
                table: "CompanyPredictions",
                column: "FinancialStatementId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyPredictions_FinancialStatements_FinancialStatementId",
                table: "CompanyPredictions",
                column: "FinancialStatementId",
                principalTable: "FinancialStatements",
                principalColumn: "FileId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
