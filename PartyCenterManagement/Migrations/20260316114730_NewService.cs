using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartyCenterManagement.Migrations
{
    /// <inheritdoc />
    public partial class NewService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "ServiceID", "Price", "Serv" },
                values: new object[] { 6, 80.0, "Photography" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "ServiceID",
                keyValue: 6);
        }
    }
}
