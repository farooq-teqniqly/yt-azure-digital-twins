using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWineRack.Migrations
{
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
