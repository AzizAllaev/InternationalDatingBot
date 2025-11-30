using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class MigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModeServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TelegramId = table.Column<long>(type: "bigint", nullable: true),
                    ModeStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetPartnerServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TelegramID = table.Column<long>(type: "bigint", nullable: false),
                    LastUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetPartnerServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TelegramID = table.Column<long>(type: "bigint", nullable: false),
                    MaleFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MalePhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaleTelegramUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaleLyceumName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaleGroupId = table.Column<int>(type: "int", nullable: true),
                    mgroupId = table.Column<int>(type: "int", nullable: true),
                    MalePurpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FemaleFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FemaleTelegramUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FemaleLyceumName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FemaleGroupId = table.Column<int>(type: "int", nullable: true),
                    fgroupId = table.Column<int>(type: "int", nullable: true),
                    FemalePurpose = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Groups_fgroupId",
                        column: x => x.fgroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Applications_Groups_mgroupId",
                        column: x => x.mgroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TelegramID = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Groups_GroupID",
                        column: x => x.GroupID,
                        principalTable: "Groups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaleId = table.Column<int>(type: "int", nullable: true),
                    FemaleId = table.Column<int>(type: "int", nullable: true),
                    SenderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_Users_FemaleId",
                        column: x => x.FemaleId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Likes_Users_MaleId",
                        column: x => x.MaleId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RegistrationStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TelegramId = table.Column<long>(type: "bigint", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: true),
                    UserRegStatus = table.Column<int>(type: "int", nullable: true),
                    AppStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrationStatuses_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_fgroupId",
                table: "Applications",
                column: "fgroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_mgroupId",
                table: "Applications",
                column: "mgroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_FemaleId",
                table: "Likes",
                column: "FemaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_MaleId",
                table: "Likes",
                column: "MaleId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationStatuses_userId",
                table: "RegistrationStatuses",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupID",
                table: "Users",
                column: "GroupID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "ModeServices");

            migrationBuilder.DropTable(
                name: "RegistrationStatuses");

            migrationBuilder.DropTable(
                name: "TargetPartnerServices");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
