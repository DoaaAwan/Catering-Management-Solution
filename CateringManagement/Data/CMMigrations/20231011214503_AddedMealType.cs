using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CateringManagement.Data.CMMigrations
{
    /// <inheritdoc />
    public partial class AddedMealType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MealTypeID",
                table: "Functions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Functions_MealTypeID",
                table: "Functions",
                column: "MealTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Functions_FunctionTypes_MealTypeID",
                table: "Functions",
                column: "MealTypeID",
                principalTable: "FunctionTypes",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Functions_FunctionTypes_MealTypeID",
                table: "Functions");

            migrationBuilder.DropIndex(
                name: "IX_Functions_MealTypeID",
                table: "Functions");

            migrationBuilder.DropColumn(
                name: "MealTypeID",
                table: "Functions");
        }
    }
}
