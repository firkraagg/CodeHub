using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHub.Migrations
{
    /// <inheritdoc />
    public partial class Added_problems_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Problems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Acceptance = table.Column<double>(type: "float", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    RequiredInput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredOutput = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Constraints = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Problems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Problems_UserId",
                table: "Problems",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Problems");
        }
    }
}
