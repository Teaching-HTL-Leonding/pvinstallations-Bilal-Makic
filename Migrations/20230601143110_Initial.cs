using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pvinstallations_Bilal_Makic.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PvInstallations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PvInstallations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProducedWattage = table.Column<float>(type: "real", nullable: false),
                    HouseholdWattage = table.Column<float>(type: "real", nullable: false),
                    BatteryWattage = table.Column<float>(type: "real", nullable: false),
                    GridWattage = table.Column<float>(type: "real", nullable: false),
                    PvInstallationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionReports_PvInstallations_PvInstallationId",
                        column: x => x.PvInstallationId,
                        principalTable: "PvInstallations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionReports_PvInstallationId",
                table: "ProductionReports",
                column: "PvInstallationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionReports");

            migrationBuilder.DropTable(
                name: "PvInstallations");
        }
    }
}
