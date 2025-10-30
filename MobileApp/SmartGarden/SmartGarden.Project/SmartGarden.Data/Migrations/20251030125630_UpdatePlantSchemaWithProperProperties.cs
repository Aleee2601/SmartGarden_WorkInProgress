using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartGarden.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlantSchemaWithProperProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plants_SoilTypes_SoilTypeId",
                table: "Plants");

            migrationBuilder.DropForeignKey(
                name: "FK_Plants_Species_SpeciesId",
                table: "Plants");

            migrationBuilder.DropForeignKey(
                name: "FK_SensorReadings_Plants_PlantId",
                table: "SensorReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_WateringLogs_Plants_PlantId",
                table: "WateringLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WateringLogs",
                table: "WateringLogs");

            migrationBuilder.DropIndex(
                name: "IX_WateringLogs_PlantId",
                table: "WateringLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorReadings",
                table: "SensorReadings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plants",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WateringLogs");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "WateringLogs");

            migrationBuilder.DropColumn(
                name: "DefaultWaterFreqDays",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "RecWaterDurSec",
                table: "SoilTypes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "SensorReadings");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "DefaultHumidMin",
                table: "Species",
                newName: "DefaultLightMin");

            migrationBuilder.RenameColumn(
                name: "DefaultHumidMax",
                table: "Species",
                newName: "DefaultLightMax");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Species",
                newName: "SpeciesId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SoilTypes",
                newName: "SoilTypeId");

            migrationBuilder.RenameColumn(
                name: "Temperature",
                table: "SensorReadings",
                newName: "WaterLevel");

            migrationBuilder.RenameColumn(
                name: "Humidity",
                table: "SensorReadings",
                newName: "AirTemp");

            migrationBuilder.AlterColumn<int>(
                name: "Mode",
                table: "WateringLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<long>(
                name: "WateringId",
                table: "WateringLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WateringLogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WateringLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "WateringLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AutoWateringEnabled",
                table: "UserSettings",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserSettings",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ScientificName",
                table: "Species",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommonName",
                table: "Species",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Species",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<double>(
                name: "DefaultHumidityMax",
                table: "Species",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DefaultHumidityMin",
                table: "Species",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Species",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Species",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PauseBetweenWaterMin",
                table: "SoilTypes",
                type: "int",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SoilTypes",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SoilTypes",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SoilTypes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SoilTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RecWaterDueSec",
                table: "SoilTypes",
                type: "int",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SoilTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "LightLevel",
                table: "SensorReadings",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "AirQuality",
                table: "SensorReadings",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ReadingId",
                table: "SensorReadings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<double>(
                name: "AirHumidity",
                table: "SensorReadings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SensorReadings",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SensorReadings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SensorReadings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Plants",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Plants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "PlantId",
                table: "Plants",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Plants",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAcquired",
                table: "Plants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Plants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOutdoor",
                table: "Plants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "Plants",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "Plants",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Plants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WateringLogs",
                table: "WateringLogs",
                column: "WateringId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorReadings",
                table: "SensorReadings",
                column: "ReadingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plants",
                table: "Plants",
                column: "PlantId");

            migrationBuilder.InsertData(
                table: "SoilTypes",
                columns: new[] { "SoilTypeId", "Description", "Name", "PauseBetweenWaterMin", "RecWaterDueSec", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Mix general pentru plante de interior", "Universal (pământ flori)", 2, 5, null },
                    { 2, "Drenaj rapid, retentie mică", "Cactuși/Suculente (nisipos)", 5, 3, null }
                });

            migrationBuilder.InsertData(
                table: "Species",
                columns: new[] { "SpeciesId", "CommonName", "DefaultHumidityMax", "DefaultHumidityMin", "DefaultLightMax", "DefaultLightMin", "DefaultSoilMoistMax", "DefaultSoilMoistMin", "DefaultTempMax", "DefaultTempMin", "ScientificName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Ficus lyrata", 70.0, 30.0, 10000.0, 500.0, 60.0, 30.0, 27.0, 18.0, "Ficus lyrata", null },
                    { 2, "Monstera deliciosa", 80.0, 40.0, 8000.0, 300.0, 65.0, 30.0, 28.0, 18.0, "Monstera deliciosa", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WateringLogs_PlantId_CreatedAt",
                table: "WateringLogs",
                columns: new[] { "PlantId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Species_ScientificName",
                table: "Species",
                column: "ScientificName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SensorReadings_PlantId_CreatedAt",
                table: "SensorReadings",
                columns: new[] { "PlantId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Plants_UserId_IsOutdoor",
                table: "Plants",
                columns: new[] { "UserId", "IsOutdoor" });

            migrationBuilder.CreateIndex(
                name: "IX_Plants_UserId_SpeciesId",
                table: "Plants",
                columns: new[] { "UserId", "SpeciesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_SoilTypes_SoilTypeId",
                table: "Plants",
                column: "SoilTypeId",
                principalTable: "SoilTypes",
                principalColumn: "SoilTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Plants_Species_SpeciesId",
                table: "Plants",
                column: "SpeciesId",
                principalTable: "Species",
                principalColumn: "SpeciesId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReadings_Plants_PlantId",
                table: "SensorReadings",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "PlantId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WateringLogs_Plants_PlantId",
                table: "WateringLogs",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "PlantId",
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
                name: "FK_SensorReadings_Plants_PlantId",
                table: "SensorReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_WateringLogs_Plants_PlantId",
                table: "WateringLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WateringLogs",
                table: "WateringLogs");

            migrationBuilder.DropIndex(
                name: "IX_WateringLogs_PlantId_CreatedAt",
                table: "WateringLogs");

            migrationBuilder.DropIndex(
                name: "IX_Species_ScientificName",
                table: "Species");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorReadings",
                table: "SensorReadings");

            migrationBuilder.DropIndex(
                name: "IX_SensorReadings_PlantId_CreatedAt",
                table: "SensorReadings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plants",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Plants_UserId_IsOutdoor",
                table: "Plants");

            migrationBuilder.DropIndex(
                name: "IX_Plants_UserId_SpeciesId",
                table: "Plants");

            migrationBuilder.DeleteData(
                table: "SoilTypes",
                keyColumn: "SoilTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SoilTypes",
                keyColumn: "SoilTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "SpeciesId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "SpeciesId",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "WateringId",
                table: "WateringLogs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WateringLogs");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WateringLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "WateringLogs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserSettings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "DefaultHumidityMax",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "DefaultHumidityMin",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Species");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SoilTypes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SoilTypes");

            migrationBuilder.DropColumn(
                name: "RecWaterDueSec",
                table: "SoilTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SoilTypes");

            migrationBuilder.DropColumn(
                name: "ReadingId",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "AirHumidity",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SensorReadings");

            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "DateAcquired",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "IsOutdoor",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Plants");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "DefaultLightMin",
                table: "Species",
                newName: "DefaultHumidMin");

            migrationBuilder.RenameColumn(
                name: "DefaultLightMax",
                table: "Species",
                newName: "DefaultHumidMax");

            migrationBuilder.RenameColumn(
                name: "SpeciesId",
                table: "Species",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "SoilTypeId",
                table: "SoilTypes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "WaterLevel",
                table: "SensorReadings",
                newName: "Temperature");

            migrationBuilder.RenameColumn(
                name: "AirTemp",
                table: "SensorReadings",
                newName: "Humidity");

            migrationBuilder.AlterColumn<string>(
                name: "Mode",
                table: "WateringLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "WateringLogs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "WateringLogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<bool>(
                name: "AutoWateringEnabled",
                table: "UserSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "ScientificName",
                table: "Species",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "CommonName",
                table: "Species",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AddColumn<int>(
                name: "DefaultWaterFreqDays",
                table: "Species",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "PauseBetweenWaterMin",
                table: "SoilTypes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SoilTypes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SoilTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecWaterDurSec",
                table: "SoilTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "LightLevel",
                table: "SensorReadings",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "AirQuality",
                table: "SensorReadings",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SensorReadings",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "SensorReadings",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Plants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Plants",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WateringLogs",
                table: "WateringLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorReadings",
                table: "SensorReadings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plants",
                table: "Plants",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WateringLogs_PlantId",
                table: "WateringLogs",
                column: "PlantId");

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
                name: "FK_SensorReadings_Plants_PlantId",
                table: "SensorReadings",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WateringLogs_Plants_PlantId",
                table: "WateringLogs",
                column: "PlantId",
                principalTable: "Plants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
