using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNU.CanteenConnect.Web.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureFinalReviewRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_FoodItems_FoodItemItemId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_FoodItemItemId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "FoodItemItemId",
                table: "Reviews");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 6, 22, 14, 16, 33, 587, DateTimeKind.Utc).AddTicks(7656), "AQAAAAIAAYagAAAAECD2Inh1dfZMzTMr2DiArzDJIW/q1cjG//36PcGLVMJnQIWukKQG8dm635g+Dht+5g==" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ItemId",
                table: "Reviews",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_FoodItems_ItemId",
                table: "Reviews",
                column: "ItemId",
                principalTable: "FoodItems",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_FoodItems_ItemId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ItemId",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "FoodItemItemId",
                table: "Reviews",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 6, 22, 13, 58, 52, 536, DateTimeKind.Utc).AddTicks(2606), "AQAAAAIAAYagAAAAEICWSjw9VZSL8nSPRFo0BN50XwbPk443JSLOjmXC+b1WjhWXL8l+mCFatdtyde0Chw==" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_FoodItemItemId",
                table: "Reviews",
                column: "FoodItemItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_FoodItems_FoodItemItemId",
                table: "Reviews",
                column: "FoodItemItemId",
                principalTable: "FoodItems",
                principalColumn: "ItemId");
        }
    }
}
