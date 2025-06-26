using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DNU.CanteenConnect.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewFunctionality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ef7d48f-2f66-41c8-b4b6-d6ef548b6623");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f5bfd8bc-8d98-40ab-9fb2-b9d5ac373a28");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "fa22a211-8387-4e64-a7d0-de2fbed1af95", "2cacb2b8-e485-46e0-a668-f4e46d40bb30" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fa22a211-8387-4e64-a7d0-de2fbed1af95");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2cacb2b8-e485-46e0-a668-f4e46d40bb30");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    FoodItemItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_FoodItems_FoodItemItemId",
                        column: x => x.FoodItemItemId,
                        principalTable: "FoodItems",
                        principalColumn: "ItemId");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a725130b-d248-4395-8178-01124e5251a1", null, "Admin", "ADMIN" },
                    { "b845130b-d248-4395-8178-01124e5251a2", null, "CanteenStaff", "CANTEENSTAFF" },
                    { "c965130b-d248-4395-8178-01124e5251a3", null, "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "CreatedDate", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d125130b-d248-4395-8178-01124e5251a4", 0, null, "f286828a-1a3b-4c4f-a719-7f51a4e21a2c", new DateTime(2025, 6, 22, 12, 36, 14, 242, DateTimeKind.Utc).AddTicks(9608), "admin@canteen.com", true, false, null, "ADMIN@CANTEEN.COM", "ADMIN@CANTEEN.COM", "AQAAAAIAAYagAAAAEIgD9SL+47yavf1dl+y2rBQJJHpAnZ/Nv+efe2S/iK1hxJdxLMH06/uI56tNweR97w==", null, false, "0a5a51c4-118d-4f11-9a74-9f20e4b868e4", false, "admin@canteen.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a725130b-d248-4395-8178-01124e5251a1", "d125130b-d248-4395-8178-01124e5251a4" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_FoodItemItemId",
                table: "Reviews",
                column: "FoodItemItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b845130b-d248-4395-8178-01124e5251a2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c965130b-d248-4395-8178-01124e5251a3");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a725130b-d248-4395-8178-01124e5251a1", "d125130b-d248-4395-8178-01124e5251a4" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a725130b-d248-4395-8178-01124e5251a1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5ef7d48f-2f66-41c8-b4b6-d6ef548b6623", null, "CanteenStaff", "CANTEENSTAFF" },
                    { "f5bfd8bc-8d98-40ab-9fb2-b9d5ac373a28", null, "Customer", "CUSTOMER" },
                    { "fa22a211-8387-4e64-a7d0-de2fbed1af95", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "CreatedDate", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "2cacb2b8-e485-46e0-a668-f4e46d40bb30", 0, null, "855ea177-7b1b-44ae-98ba-25c0c1378b71", new DateTime(2025, 6, 17, 8, 23, 43, 866, DateTimeKind.Utc).AddTicks(9227), "admin@canteen.com", true, false, null, "ADMIN@CANTEEN.COM", "ADMIN@CANTEEN.COM", "AQAAAAIAAYagAAAAEEbWgHwLsHGR8ql+exmqyFzJr5IWhdqezaujZx3PsmQemWmC+K8x3yyl+TgFEqhBrQ==", null, false, "43d00ec9-dc5c-49e8-a247-2a401f8829d8", false, "admin@canteen.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "fa22a211-8387-4e64-a7d0-de2fbed1af95", "2cacb2b8-e485-46e0-a668-f4e46d40bb30" });
        }
    }
}
