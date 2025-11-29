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
            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "FoodItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 29, 12, 38, 46, 226, DateTimeKind.Utc).AddTicks(3466), "AQAAAAIAAYagAAAAEO83mxMahTpLmO8b3xZ4tUCGUIh1+vYXNEwGboYpy+8oup4zlzQexj386p1myUBuGA==" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "FoodItems");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 2, 15, 36, 23, 317, DateTimeKind.Utc).AddTicks(3970), "AQAAAAIAAYagAAAAELS2GPBQiWOMKeM0JiofIJUpgaxnUlO8haHGTCzphar9iH42WY/oZEIlXu4YLdjzAg==" });
        }
    }
}
