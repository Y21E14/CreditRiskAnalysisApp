using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditRiskAnalysisApp.Migrations
{
    public partial class CreateCompanyPredictionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create the CompanyPredictions table with all required columns
            migrationBuilder.CreateTable(
                name: "CompanyPredictions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(nullable: false),
                    CreditRisk = table.Column<string>(nullable: true),
                    CreditRiskNumerical = table.Column<int>(nullable: false),
                    DebtServiceCoverageRatio = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    DebtToEquityRatio = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    GrossProfitMargin = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    WorkingCapitalRatio = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PredictionDate = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPredictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyPredictions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CompanyPredictions");
        }
    }
}
