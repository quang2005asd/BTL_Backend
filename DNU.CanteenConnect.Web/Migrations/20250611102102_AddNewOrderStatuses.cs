using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNU.CanteenConnect.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddNewOrderStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJ1RwQQjBcwdRzrichmWU2w16TmY9dDf68+JwxFp4Pe8CGn+EGCkt+qHL1By5k2lhw==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELDYQ7GjjBzlSliznrokhMasVXh21cDU9YcWZ2s3kJxFFEneapztESVPcDMubIeScw==");
        }
    }
}
