using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class initialDbCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RatedPowerWatts = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: true),
                    DeviceId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TriggeredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Alerts_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DeviceStateHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    IsOn = table.Column<bool>(type: "bit", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceStateHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceStateHistories_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceStates",
                columns: table => new
                {
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    IsOn = table.Column<bool>(type: "bit", nullable: false),
                    LastChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceStates", x => x.DeviceId);
                    table.ForeignKey(
                        name: "FK_DeviceStates_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Code", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "drawing", "Drawing Room", 1 },
                    { 2, "work1", "Work Room 1", 2 },
                    { 3, "work2", "Work Room 2", 2 }
                });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "Name", "RatedPowerWatts", "RoomId", "Type" },
                values: new object[,]
                {
                    { 1, "Fan 1", 60, 1, 1 },
                    { 2, "Fan 2", 60, 1, 1 },
                    { 3, "Light 1", 15, 1, 2 },
                    { 4, "Light 2", 15, 1, 2 },
                    { 5, "Light 3", 15, 1, 2 },
                    { 6, "Fan 1", 60, 2, 1 },
                    { 7, "Fan 2", 60, 2, 1 },
                    { 8, "Light 1", 15, 2, 2 },
                    { 9, "Light 2", 15, 2, 2 },
                    { 10, "Light 3", 15, 2, 2 },
                    { 11, "Fan 1", 60, 3, 1 },
                    { 12, "Fan 2", 60, 3, 1 },
                    { 13, "Light 1", 15, 3, 2 },
                    { 14, "Light 2", 15, 3, 2 },
                    { 15, "Light 3", 15, 3, 2 }
                });

            migrationBuilder.InsertData(
                table: "DeviceStates",
                columns: new[] { "DeviceId", "IsOn", "LastChangedAt" },
                values: new object[,]
                {
                    { 1, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, false, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_DeviceId",
                table: "Alerts",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_ResolvedAt_TriggeredAt",
                table: "Alerts",
                columns: new[] { "ResolvedAt", "TriggeredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_RoomId",
                table: "Alerts",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_RoomId",
                table: "Devices",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceStateHistories_DeviceId_ChangedAt",
                table: "DeviceStateHistories",
                columns: new[] { "DeviceId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Code",
                table: "Rooms",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "DeviceStateHistories");

            migrationBuilder.DropTable(
                name: "DeviceStates");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
