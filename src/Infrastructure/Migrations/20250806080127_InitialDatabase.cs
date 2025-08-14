using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    username = table.Column<string>(type: "VARCHAR2(50 CHAR)", nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP(0)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
