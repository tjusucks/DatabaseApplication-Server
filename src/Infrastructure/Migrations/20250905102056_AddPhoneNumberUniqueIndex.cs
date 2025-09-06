using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clean up duplicate phone numbers before adding unique constraint
            migrationBuilder.Sql(@"
                UPDATE ""users""
                SET ""phone_number"" = ""phone_number"" || '_' || ""user_id""
                WHERE ""phone_number"" IS NOT NULL
                AND ""phone_number"" IN (
                    SELECT ""phone_number""
                    FROM ""users""
                    WHERE ""phone_number"" IS NOT NULL
                    GROUP BY ""phone_number""
                    HAVING COUNT(*) > 1
                )
            ");

            migrationBuilder.CreateIndex(
                name: "IX_users_phone_number",
                table: "users",
                column: "phone_number",
                unique: true,
                filter: "phone_number IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_phone_number",
                table: "users");
        }
    }
}
