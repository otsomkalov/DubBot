using Microsoft.EntityFrameworkCore.Migrations;

namespace Bot.Migrations
{
    public partial class SplitwiseId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SplitwiseId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SplitwiseId",
                table: "Users");
        }
    }
}
