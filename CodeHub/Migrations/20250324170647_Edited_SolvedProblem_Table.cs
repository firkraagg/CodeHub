using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHub.Migrations
{
    /// <inheritdoc />
    public partial class Edited_SolvedProblem_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolvedProblems_Users_userId",
                table: "SolvedProblems");

            migrationBuilder.AddColumn<DateTime>(
                name: "SolvedAt",
                table: "SolvedProblems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SourceCode",
                table: "SolvedProblems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_SolvedProblems_Users_userId",
                table: "SolvedProblems",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolvedProblems_Users_userId",
                table: "SolvedProblems");

            migrationBuilder.DropColumn(
                name: "SolvedAt",
                table: "SolvedProblems");

            migrationBuilder.DropColumn(
                name: "SourceCode",
                table: "SolvedProblems");

            migrationBuilder.AddForeignKey(
                name: "FK_SolvedProblems_Users_userId",
                table: "SolvedProblems",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
