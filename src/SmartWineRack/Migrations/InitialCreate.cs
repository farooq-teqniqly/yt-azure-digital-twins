// <copyright file="20221228002439_InitialCreate.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

#nullable disable

namespace SmartWineRack.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WineRack",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "current_timestamp"),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "current_timestamp"),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WineRack", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WineRackConfig",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "current_timestamp"),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "current_timestamp"),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WineRackConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scanner",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "current_timestamp"),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "current_timestamp"),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scanner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scanner_WineRack_Id",
                        column: x => x.Id,
                        principalTable: "WineRack",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WineRackSlot",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    SlotNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "current_timestamp"),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "current_timestamp"),
                    WineRackId = table.Column<string>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WineRackSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WineRackSlot_WineRack_WineRackId",
                        column: x => x.WineRackId,
                        principalTable: "WineRack",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bottle",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    UpcCode = table.Column<string>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "current_timestamp"),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "current_timestamp"),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bottle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bottle_WineRackSlot_Id",
                        column: x => x.Id,
                        principalTable: "WineRackSlot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "AK_Config",
                table: "WineRackConfig",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WineRackSlot_WineRackId",
                table: "WineRackSlot",
                column: "WineRackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bottle");

            migrationBuilder.DropTable(
                name: "Scanner");

            migrationBuilder.DropTable(
                name: "WineRackConfig");

            migrationBuilder.DropTable(
                name: "WineRackSlot");

            migrationBuilder.DropTable(
                name: "WineRack");
        }
    }
}
