using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIResumeScreeningSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddJobSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 19, 39, 50, 390, DateTimeKind.Utc).AddTicks(8813));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 19, 39, 50, 390, DateTimeKind.Utc).AddTicks(8815));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 19, 39, 50, 390, DateTimeKind.Utc).AddTicks(8817));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 19, 39, 50, 390, DateTimeKind.Utc).AddTicks(8819));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 19, 39, 50, 390, DateTimeKind.Utc).AddTicks(8820));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 19, 39, 50, 390, DateTimeKind.Utc).AddTicks(8822));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 19, 39, 50, 390, DateTimeKind.Utc).AddTicks(8823));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 19, 39, 50, 390, DateTimeKind.Utc).AddTicks(8825));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 19, 39, 50, 390, DateTimeKind.Utc).AddTicks(8827));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 19, 39, 50, 390, DateTimeKind.Utc).AddTicks(8828));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 18, 32, 39, 989, DateTimeKind.Utc).AddTicks(8822));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 18, 32, 39, 989, DateTimeKind.Utc).AddTicks(8825));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 18, 32, 39, 989, DateTimeKind.Utc).AddTicks(8827));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 18, 32, 39, 989, DateTimeKind.Utc).AddTicks(8829));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 18, 32, 39, 989, DateTimeKind.Utc).AddTicks(8831));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 18, 32, 39, 989, DateTimeKind.Utc).AddTicks(8833));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 18, 32, 39, 989, DateTimeKind.Utc).AddTicks(8835));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 18, 32, 39, 989, DateTimeKind.Utc).AddTicks(8837));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 18, 32, 39, 989, DateTimeKind.Utc).AddTicks(8838));

            migrationBuilder.UpdateData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 4, 18, 32, 39, 989, DateTimeKind.Utc).AddTicks(8840));
        }
    }
}
