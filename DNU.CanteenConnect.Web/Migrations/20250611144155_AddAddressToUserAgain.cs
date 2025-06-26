using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNU.CanteenConnect.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToUserAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                columns: new[] { "Address", "PasswordHash" },
                values: new object[] { null, "AQAAAAIAAYagAAAAELYTmhY3r8EkleYDiTLH0xttkRkCmtMryT04SEv2F/e9L5wAhAH5KVkKcYky3Cw5SA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d125130b-d248-4395-8178-01124e5251a4",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJ1RwQQjBcwdRzrichmWU2w16TmY9dDf68+JwxFp4Pe8CGn+EGCkt+qHL1By5k2lhw==");
        }
    }
}
