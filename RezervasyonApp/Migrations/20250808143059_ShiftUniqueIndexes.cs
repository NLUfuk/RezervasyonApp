using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RezervasyonApp.Migrations
{
    /// <inheritdoc />
    public partial class ShiftUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shifts_DriverId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_VehicleId",
                table: "Shifts");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_DriverId_DepartureTime",
                table: "Shifts",
                columns: new[] { "DriverId", "DepartureTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_VehicleId_DepartureTime",
                table: "Shifts",
                columns: new[] { "VehicleId", "DepartureTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shifts_DriverId_DepartureTime",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_VehicleId_DepartureTime",
                table: "Shifts");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_DriverId",
                table: "Shifts",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_VehicleId",
                table: "Shifts",
                column: "VehicleId");
        }
    }
}
