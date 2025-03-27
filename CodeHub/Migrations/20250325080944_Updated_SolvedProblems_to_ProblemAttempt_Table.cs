using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHub.Migrations
{
    /// <inheritdoc />
    public partial class Updated_SolvedProblems_to_ProblemAttempt_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolvedProblems");

            migrationBuilder.CreateTable(
                name: "ProblemAttempts",
                columns: table => new
                {
                    problemId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    AttemptedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemAttempts", x => new { x.userId, x.problemId });
                    table.ForeignKey(
                        name: "FK_ProblemAttempts_Problems_problemId",
                        column: x => x.problemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemAttempts_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProblemAttempts_problemId",
                table: "ProblemAttempts",
                column: "problemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProblemAttempts");

            migrationBuilder.CreateTable(
                name: "SolvedProblems",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false),
                    problemId = table.Column<int>(type: "int", nullable: false),
                    SolvedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SourceCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolvedProblems", x => new { x.userId, x.problemId });
                    table.ForeignKey(
                        name: "FK_SolvedProblems_Problems_problemId",
                        column: x => x.problemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolvedProblems_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolvedProblems_problemId",
                table: "SolvedProblems",
                column: "problemId");
        }
    }
}
