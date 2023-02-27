using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZME.API.Migrations
{
    /// <inheritdoc />
    public partial class RoleEmailAndSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Roles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Email", "Name", "NameShort" },
                values: new object[,]
                {
                    { 1, "atm@memphisartcc.com", "Air Traffic Manager", "ATM" },
                    { 2, "datm@memphisartcc.com", "Deputy Air Traffic Manager", "DATM" },
                    { 3, "ta@memphisartcc.com", "Training Administrator", "TA" },
                    { 4, "ata@memphisartcc.com", "Assistant Training Administrator", "ATA" },
                    { 5, "wm@memphisartcc.com", "Webmaster", "WM" },
                    { 6, "awm@memphisartcc.com", "Assistant Webmaster", "AWM" },
                    { 7, "ec@memphisartcc.com", "Events Coordinator", "EC" },
                    { 8, "aec@memphisartcc.com", "Assistant Events Coordinator", "AEC" },
                    { 9, "fe@memphisartcc.com", "Facility Engineer", "FE" },
                    { 10, "afe@memphisartcc.com", "Assistant Facility Engineer", "AFE" },
                    { 11, "instructors@memphisartcc.com", "Instructor", "INS" },
                    { 12, "mentors@memphisartcc.com", "Mentor", "MTR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Roles");
        }
    }
}
