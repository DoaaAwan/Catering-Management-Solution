using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CateringManagement.Data.CMMigrations
{
    /// <inheritdoc />
    public partial class AddedEndTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Functions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Functions");
        }
    }
}
