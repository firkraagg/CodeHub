using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeHub.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Problem_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultCode",
                table: "Problems",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCode",
                table: "Problems");
        }
    }
}
