using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditRiskAnalysisApp.Migrations
{
    /// <inheritdoc />
    public partial class CleanUpIndexesAndColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchivedPredictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreditRisk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreditRiskNumerical = table.Column<int>(type: "int", nullable: false),
                    DebtServiceCoverageRatio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DebtToEquityRatio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossProfitMargin = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    WorkingCapitalRatio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PredictionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedPredictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivedPredictions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedPredictions_CompanyId",
                table: "ArchivedPredictions",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedPredictions");
        }
    }
}
