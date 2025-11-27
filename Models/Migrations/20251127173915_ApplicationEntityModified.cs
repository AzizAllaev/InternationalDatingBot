using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationEntityModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppStatus",
                table: "RegistrationStatuses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FemaleTelegramUser",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MalePhoneNumber",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaleTelegramUser",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppStatus",
                table: "RegistrationStatuses");

            migrationBuilder.DropColumn(
                name: "FemaleTelegramUser",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "MalePhoneNumber",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "MaleTelegramUser",
                table: "Applications");
        }
    }
}
