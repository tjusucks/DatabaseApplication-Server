using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRideEntryRecordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ride_entry_records",
                columns: table => new
                {
                    ride_entry_record_id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    visitor_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ride_id = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    entry_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    exit_time = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    entry_gate = table.Column<string>(type: "VARCHAR2(30 CHAR)", nullable: false),
                    exit_gate = table.Column<string>(type: "VARCHAR2(30 CHAR)", nullable: true),
                    ticket_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "SYSTIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "TIMESTAMP", nullable: true, defaultValueSql: "SYSTIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ride_entry_records", x => x.ride_entry_record_id);
                    table.ForeignKey(
                        name: "FK_ride_entry_records_amusement_rides_ride_id",
                        column: x => x.ride_id,
                        principalTable: "amusement_rides",
                        principalColumn: "ride_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ride_entry_records_tickets_ticket_id",
                        column: x => x.ticket_id,
                        principalTable: "tickets",
                        principalColumn: "ticket_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ride_entry_records_visitors_visitor_id",
                        column: x => x.visitor_id,
                        principalTable: "visitors",
                        principalColumn: "visitor_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ride_entry_records_entry_time",
                table: "ride_entry_records",
                column: "entry_time");

            migrationBuilder.CreateIndex(
                name: "IX_ride_entry_records_exit_time",
                table: "ride_entry_records",
                column: "exit_time");

            migrationBuilder.CreateIndex(
                name: "IX_ride_entry_records_ride_id",
                table: "ride_entry_records",
                column: "ride_id");

            migrationBuilder.CreateIndex(
                name: "IX_ride_entry_records_ticket_id",
                table: "ride_entry_records",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_ride_entry_records_visitor_id",
                table: "ride_entry_records",
                column: "visitor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ride_entry_records");
        }
    }
}
