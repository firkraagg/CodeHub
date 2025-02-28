using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHub.Migrations
{
    /// <inheritdoc />
    public partial class ChangedProgramming_Language_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiName",
                table: "ProgrammingLanguage");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "ProgrammingLanguage",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "DockerImage",
                table: "ProgrammingLanguage",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DockerImage",
                table: "ProgrammingLanguage");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProgrammingLanguage",
                newName: "DisplayName");

            migrationBuilder.AddColumn<string>(
                name: "ApiName",
                table: "ProgrammingLanguage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
