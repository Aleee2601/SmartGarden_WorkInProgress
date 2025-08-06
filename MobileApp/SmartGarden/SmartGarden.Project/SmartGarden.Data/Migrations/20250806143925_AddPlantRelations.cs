using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartGarden.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationSec",
                table: "WateringLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "AirQuality",
                table: "SensorReadings",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LightLevel",
                table: "SensorReadings",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoilTypeId",
                table: "Plants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpeciesId",
                table: "Plants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Plants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SoilTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecWaterDurSec = table.Column<int>(type: "int", nullable: false),
                    PauseBetweenWaterMin = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoilTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommonName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScientificName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultWaterFreqDays = table.Column<int>(type: "int", nullable: false),
                    DefaultSoilMoistMin = table.Column<double>(type: "float", nullable: false),
                    DefaultSoilMoistMax = table.Column<double>(type: "float", nullable: false),
                    DefaultTempMin = table.Column<double>(type: "float", nullable: false),
                    DefaultTempMax = table.Column<double>(type: "float", nullable: false),
                    DefaultHumidMin = table.Column<double>(type: "float", nullable: false),
                    DefaultHumidMax = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AutoWateringEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SoilMoistThreshold = table.Column<double>(type: "float", nullable: false, defaultValue: 30.0),
                    DataReadIntervalMin = table.Column<int>(type: "int", nullable: false, defaultValue: 15)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plants_SoilTypeId",
                table: "Plants",
                column: "SoilTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_SpeciesId",
                table: "Plants",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_UserId",
                table: "Plants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_SoilTypes_SoilTypeId",
                table: "Plants",
                column: "SoilTypeId",
                principalTable: "SoilTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_Species_SpeciesId",
                table: "Plants",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_Users_UserId",
                table: "Plants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_SoilTypes_SoilTypeId",
                table: "Plants");

            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Species_SpeciesId",
                table: "Plants");

            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Users_UserId",
                table: "Plants");

            migrationBuilder.DropTable(
                name: "SoilTypes");

            migrationBuilder.DropTable(
                name: "Species");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Plants_SoilTypeId",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Plants_SpeciesId",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Plants_UserId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "DurationSec",
                table: "WateringLogs");

            migrationBuilder.DropColumn(
                name: "AirQuality",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "LightLevel",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "SoilTypeId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "SpeciesId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Plants");
        }
    }
}
