using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNU.CanteenConnect.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFoodItemSeedDataWithStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 3, 13, 23, 54, 48, DateTimeKind.Utc).AddTicks(4918), "AQAAAAIAAYagAAAAEEJ9NvpkTNOOiQqlEQmYNusDLl6IZNYh/SgEpfk1LVJTZY3uM1e6vy3ypfa+lTAIvA==" });

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 1,
                column: "StockQuantity",
                value: 50);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 2,
                column: "StockQuantity",
                value: 60);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 3,
                column: "StockQuantity",
                value: 100);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 4,
                column: "StockQuantity",
                value: 80);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 5,
                column: "StockQuantity",
                value: 40);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 6,
                column: "StockQuantity",
                value: 120);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 7,
                column: "StockQuantity",
                value: 45);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 8,
                column: "StockQuantity",
                value: 55);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 1, 16, 43, 32, 656, DateTimeKind.Utc).AddTicks(2481), "AQAAAAIAAYagAAAAEIPPfuMMFNg+l/csSiCKYJKTpxwlwkTGQ17pBhR56YQGwoe7g4N0eEDIwExfMgjOyA==" });

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 1,
                column: "StockQuantity",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 2,
                column: "StockQuantity",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 3,
                column: "StockQuantity",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 4,
                column: "StockQuantity",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 5,
                column: "StockQuantity",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 6,
                column: "StockQuantity",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 7,
                column: "StockQuantity",
                value: 0);

            migrationBuilder.UpdateData(
                table: "FoodItems",
                keyColumn: "ItemId",
                keyValue: 8,
                column: "StockQuantity",
                value: 0);
        }
    }
}
