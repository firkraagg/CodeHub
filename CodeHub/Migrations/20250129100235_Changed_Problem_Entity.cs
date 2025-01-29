using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHub.Migrations
{
    /// <inheritdoc />
    public partial class Changed_Problem_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Users_UserId",
                table: "Problems");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Problems",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Problems_UserId",
                table: "Problems",
                newName: "IX_Problems_UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Users_UserID",
                table: "Problems",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_Users_UserID",
                table: "Problems");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Problems",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Problems_UserID",
                table: "Problems",
                newName: "IX_Problems_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_Users_UserId",
                table: "Problems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
