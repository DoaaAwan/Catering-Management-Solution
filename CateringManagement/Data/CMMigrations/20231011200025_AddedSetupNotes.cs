using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CateringManagement.Data.CMMigrations
{
    /// <inheritdoc />
    public partial class AddedSetupNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SetupNotes",
                table: "Functions",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetupNotes",
                table: "Functions");
        }
    }
}
