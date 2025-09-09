using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToFinancialTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "salary_records",
                type: "NVARCHAR2(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "financial_records",
                type: "NVARCHAR2(500)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "notes",
                table: "salary_records");

            migrationBuilder.DropColumn(
                name: "description",
                table: "financial_records");
        }
    }
}
