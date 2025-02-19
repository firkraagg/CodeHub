using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHub.Migrations
{
    /// <inheritdoc />
    public partial class RenameLanguageNameToApiName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LanguageName",
                table: "ProgrammingLanguage",
                newName: "Version");

            migrationBuilder.AddColumn<string>(
                name: "ApiName",
                table: "ProgrammingLanguage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "ProgrammingLanguage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiName",
                table: "ProgrammingLanguage");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "ProgrammingLanguage");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "ProgrammingLanguage",
                newName: "LanguageName");
        }
    }
}
