using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNU.CanteenConnect.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddStockQuantityToFoodItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 1, 16, 43, 32, 656, DateTimeKind.Utc).AddTicks(2481), "AQAAAAIAAYagAAAAEIPPfuMMFNg+l/csSiCKYJKTpxwlwkTGQ17pBhR56YQGwoe7g4N0eEDIwExfMgjOyA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 1, 16, 43, 3, 360, DateTimeKind.Utc).AddTicks(8953), "AQAAAAIAAYagAAAAEL2T5pEcShNoWnWy76BJ96z0g2rzyBwmdQTsrop/3N/0JyPe4Q6u7ORDnAfUGy0cRA==" });
        }
    }
}
