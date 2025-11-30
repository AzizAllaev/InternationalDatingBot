using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class LittleChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatuses_Users_userId",
                table: "RegistrationStatuses");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "RegistrationStatuses",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrationStatuses_userId",
                table: "RegistrationStatuses",
                newName: "IX_RegistrationStatuses_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatuses_Users_UserId",
                table: "RegistrationStatuses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationStatuses_Users_UserId",
                table: "RegistrationStatuses");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RegistrationStatuses",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrationStatuses_UserId",
                table: "RegistrationStatuses",
                newName: "IX_RegistrationStatuses_userId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationStatuses_Users_userId",
                table: "RegistrationStatuses",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
