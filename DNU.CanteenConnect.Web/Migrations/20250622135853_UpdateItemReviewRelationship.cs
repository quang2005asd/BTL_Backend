using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNU.CanteenConnect.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateItemReviewRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 6, 22, 13, 58, 52, 536, DateTimeKind.Utc).AddTicks(2606), "AQAAAAIAAYagAAAAEICWSjw9VZSL8nSPRFo0BN50XwbPk443JSLOjmXC+b1WjhWXL8l+mCFatdtyde0Chw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 6, 22, 12, 36, 14, 242, DateTimeKind.Utc).AddTicks(9608), "AQAAAAIAAYagAAAAEIgD9SL+47yavf1dl+y2rBQJJHpAnZ/Nv+efe2S/iK1hxJdxLMH06/uI56tNweR97w==" });
        }
    }
}
