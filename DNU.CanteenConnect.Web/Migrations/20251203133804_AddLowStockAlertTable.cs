using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DNU.CanteenConnect.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddLowStockAlertTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LowStockAlerts",
                columns: table => new
                {
                    AlertId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FoodItemId = table.Column<int>(type: "integer", nullable: false),
                    CanteenId = table.Column<int>(type: "integer", nullable: false),
                    CurrentStock = table.Column<int>(type: "integer", nullable: false),
                    ThresholdStock = table.Column<int>(type: "integer", nullable: false),
                    AlertStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LowStockAlerts", x => x.AlertId);
                    table.ForeignKey(
                        name: "FK_LowStockAlerts_Canteens_CanteenId",
                        column: x => x.CanteenId,
                        principalTable: "Canteens",
                        principalColumn: "CanteenId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LowStockAlerts_FoodItems_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "FoodItems",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 3, 13, 38, 1, 81, DateTimeKind.Utc).AddTicks(4502), "AQAAAAIAAYagAAAAELL/rwXRtxe6k8FLGJiSG/Cds3foCWOTHlW2fNt19EsOhpa5RbA3gqKUti9dvbCGkw==" });

            migrationBuilder.CreateIndex(
                name: "IX_LowStockAlerts_CanteenId_AlertStatus_CreatedAt",
                table: "LowStockAlerts",
                columns: new[] { "CanteenId", "AlertStatus", "CreatedAt" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_LowStockAlerts_FoodItemId_CreatedAt",
                table: "LowStockAlerts",
                columns: new[] { "FoodItemId", "CreatedAt" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LowStockAlerts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 3, 13, 28, 48, 565, DateTimeKind.Utc).AddTicks(9150), "AQAAAAIAAYagAAAAEDbBQvheNbF7UBOl/0KnHcc6xmigz2mm2S1pHv14Ez+EdQ1+U5MHhsdpOucTsnQ/VQ==" });
        }
    }
}
