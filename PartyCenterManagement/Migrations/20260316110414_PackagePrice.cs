using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartyCenterManagement.Migrations
{
    /// <inheritdoc />
    public partial class PackagePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Packages",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageID",
                keyValue: 1,
                column: "Price",
                value: 150.0);

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageID",
                keyValue: 2,
                column: "Price",
                value: 250.0);

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageID",
                keyValue: 3,
                column: "Price",
                value: 400.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Packages",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageID",
                keyValue: 1,
                column: "Price",
                value: 150);

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageID",
                keyValue: 2,
                column: "Price",
                value: 250);

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "PackageID",
                keyValue: 3,
                column: "Price",
                value: 400);
        }
    }
}
