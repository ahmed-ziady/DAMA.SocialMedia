using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAMA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FriendAndConnectionManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequest_Receiver",
                table: "FriendRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequest_Sender",
                table: "FriendRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_User1",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_User2",
                table: "Friendship");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Friendsh__4D531A74000D08A6",
                table: "Friendship");

            migrationBuilder.DropPrimaryKey(
                name: "PK__FriendRe__33A8519AF8BFE7A4",
                table: "FriendRequest");

            migrationBuilder.DropColumn(
                name: "DateBecameFriends",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "FriendshipStatus",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FriendRequest");

            migrationBuilder.RenameTable(
                name: "Friendship",
                newName: "Friendships");

            migrationBuilder.RenameTable(
                name: "FriendRequest",
                newName: "FriendRequests");

            migrationBuilder.RenameColumn(
                name: "UserID2",
                table: "Friendships",
                newName: "UserId2");

            migrationBuilder.RenameColumn(
                name: "UserID1",
                table: "Friendships",
                newName: "UserId1");

            migrationBuilder.RenameColumn(
                name: "FriendshipID",
                table: "Friendships",
                newName: "FriendshipId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_UserID2",
                table: "Friendships",
                newName: "IX_Friendships_UserId2");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_UserID1",
                table: "Friendships",
                newName: "IX_Friendships_UserId1");

            migrationBuilder.RenameColumn(
                name: "SenderID",
                table: "FriendRequests",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "ReceiverID",
                table: "FriendRequests",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "RequestID",
                table: "FriendRequests",
                newName: "FriendRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequest_SenderID",
                table: "FriendRequests",
                newName: "IX_FriendRequests_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequest_ReceiverID",
                table: "FriendRequests",
                newName: "IX_FriendRequests_ReceiverId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Friendships",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId3",
                table: "Friendships",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId4",
                table: "Friendships",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequestStatus",
                table: "FriendRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSent",
                table: "FriendRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships",
                column: "FriendshipId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendRequests",
                table: "FriendRequests",
                column: "FriendRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId3",
                table: "Friendships",
                column: "UserId3");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId4",
                table: "Friendships",
                column: "UserId4");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_User_ReceiverId",
                table: "FriendRequests",
                column: "ReceiverId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequests_User_SenderId",
                table: "FriendRequests",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_UserId",
                table: "Friendships",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_UserId1",
                table: "Friendships",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_UserId2",
                table: "Friendships",
                column: "UserId2",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_UserId3",
                table: "Friendships",
                column: "UserId3",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_UserId4",
                table: "Friendships",
                column: "UserId4",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_User_ReceiverId",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FriendRequests_User_SenderId",
                table: "FriendRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_UserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_UserId1",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_UserId2",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_UserId3",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_UserId4",
                table: "Friendships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId3",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId4",
                table: "Friendships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendRequests",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "UserId3",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "UserId4",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "DateSent",
                table: "FriendRequests");

            migrationBuilder.RenameTable(
                name: "Friendships",
                newName: "Friendship");

            migrationBuilder.RenameTable(
                name: "FriendRequests",
                newName: "FriendRequest");

            migrationBuilder.RenameColumn(
                name: "UserId2",
                table: "Friendship",
                newName: "UserID2");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "Friendship",
                newName: "UserID1");

            migrationBuilder.RenameColumn(
                name: "FriendshipId",
                table: "Friendship",
                newName: "FriendshipID");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_UserId2",
                table: "Friendship",
                newName: "IX_Friendship_UserID2");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_UserId1",
                table: "Friendship",
                newName: "IX_Friendship_UserID1");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "FriendRequest",
                newName: "SenderID");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "FriendRequest",
                newName: "ReceiverID");

            migrationBuilder.RenameColumn(
                name: "FriendRequestId",
                table: "FriendRequest",
                newName: "RequestID");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequests_SenderId",
                table: "FriendRequest",
                newName: "IX_FriendRequest_SenderID");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequests_ReceiverId",
                table: "FriendRequest",
                newName: "IX_FriendRequest_ReceiverID");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateBecameFriends",
                table: "Friendship",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<string>(
                name: "FriendshipStatus",
                table: "Friendship",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestStatus",
                table: "FriendRequest",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FriendRequest",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Friendsh__4D531A74000D08A6",
                table: "Friendship",
                column: "FriendshipID");

            migrationBuilder.AddPrimaryKey(
                name: "PK__FriendRe__33A8519AF8BFE7A4",
                table: "FriendRequest",
                column: "RequestID");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequest_Receiver",
                table: "FriendRequest",
                column: "ReceiverID",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendRequest_Sender",
                table: "FriendRequest",
                column: "SenderID",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_User1",
                table: "Friendship",
                column: "UserID1",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_User2",
                table: "Friendship",
                column: "UserID2",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
