using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAMA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Friendships");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Friendships",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Friendships",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
