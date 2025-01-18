using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditRiskAnalysisApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNRICToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NRIC",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NRIC",
                table: "Users");
        }
    }
}
