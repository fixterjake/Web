using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZME.API.Migrations
{
    /// <inheritdoc />
    public partial class UserCanStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanRegisterForEvents",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanRequestTraining",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanRegisterForEvents",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CanRequestTraining",
                table: "Users");
        }
    }
}
