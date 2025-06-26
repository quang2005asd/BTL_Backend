using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNU.CanteenConnect.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedDateToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 6, 11, 14, 51, 43, 973, DateTimeKind.Utc).AddTicks(6414), "AQAAAAIAAYagAAAAEEfUgMYQ6D19IgSUnu3l2mDUiv+dL4HaPGBqVALL4r67h37lBod6eH5tPc9y1pk5HQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELYTmhY3r8EkleYDiTLH0xttkRkCmtMryT04SEv2F/e9L5wAhAH5KVkKcYky3Cw5SA==");
        }
    }
}
