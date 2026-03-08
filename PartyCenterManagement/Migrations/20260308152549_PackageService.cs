using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PartyCenterManagement.Migrations
{
    /// <inheritdoc />
    public partial class PackageService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PackageServices",
                columns: new[] { "PackageID", "ServiceID" },
                values: new object[,]
                {
                    { 1, 3 },
                    { 2, 1 },
                    { 2, 3 },
                    { 3, 1 },
                    { 3, 3 },
                    { 3, 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PackageServices",
                keyColumns: new[] { "PackageID", "ServiceID" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "PackageServices",
                keyColumns: new[] { "PackageID", "ServiceID" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "PackageServices",
                keyColumns: new[] { "PackageID", "ServiceID" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "PackageServices",
                keyColumns: new[] { "PackageID", "ServiceID" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "PackageServices",
                keyColumns: new[] { "PackageID", "ServiceID" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "PackageServices",
                keyColumns: new[] { "PackageID", "ServiceID" },
                keyValues: new object[] { 3, 4 });
        }
    }
}
