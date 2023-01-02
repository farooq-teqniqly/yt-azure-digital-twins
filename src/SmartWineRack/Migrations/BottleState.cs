// <copyright file="20221228191654_BottleState.cs" company="Teqniqly">
// Copyright (c) Teqniqly. All rights reserved.
// </copyright>

#nullable disable

namespace SmartWineRack.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class BottleState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BottleState",
                table: "Bottle",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BottleState",
                table: "Bottle");
        }
    }
}
