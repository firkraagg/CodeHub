using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHub.Migrations
{
    /// <inheritdoc />
    public partial class Added_programmingLanguage_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Problems");

            migrationBuilder.AddColumn<int>(
                name: "LanguageID",
                table: "Problems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProgrammingLanguage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammingLanguage", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Problems_LanguageID",
                table: "Problems",
                column: "LanguageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Problems_ProgrammingLanguage_LanguageID",
                table: "Problems",
                column: "LanguageID",
                principalTable: "ProgrammingLanguage",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Problems_ProgrammingLanguage_LanguageID",
                table: "Problems");

            migrationBuilder.DropTable(
                name: "ProgrammingLanguage");

            migrationBuilder.DropIndex(
                name: "IX_Problems_LanguageID",
                table: "Problems");

            migrationBuilder.DropColumn(
                name: "LanguageID",
                table: "Problems");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Problems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
