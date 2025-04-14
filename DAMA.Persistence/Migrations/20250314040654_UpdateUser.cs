using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAMA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_User_UserId",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_User_UserId1",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_User_UserId2",
                table: "Friendship");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_UserId",
                table: "Friendship");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_UserId1",
                table: "Friendship");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_UserId2",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "UserId2",
                table: "Friendship");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Friendship",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Friendship",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId2",
                table: "Friendship",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserId",
                table: "Friendship",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserId1",
                table: "Friendship",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_UserId2",
                table: "Friendship",
                column: "UserId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_User_UserId",
                table: "Friendship",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_User_UserId1",
                table: "Friendship",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_User_UserId2",
                table: "Friendship",
                column: "UserId2",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
