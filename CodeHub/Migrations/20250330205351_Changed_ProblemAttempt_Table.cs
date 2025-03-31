using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHub.Migrations
{
    /// <inheritdoc />
    public partial class Changed_ProblemAttempt_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProblemAttempts",
                table: "ProblemAttempts");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProblemAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProblemAttempts",
                table: "ProblemAttempts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemAttempts_userId",
                table: "ProblemAttempts",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProblemAttempts",
                table: "ProblemAttempts");

            migrationBuilder.DropIndex(
                name: "IX_ProblemAttempts_userId",
                table: "ProblemAttempts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProblemAttempts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProblemAttempts",
                table: "ProblemAttempts",
                columns: new[] { "userId", "problemId" });
        }
    }
}
