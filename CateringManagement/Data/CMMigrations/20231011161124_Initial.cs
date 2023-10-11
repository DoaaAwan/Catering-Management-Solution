using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CateringManagement.Data.CMMigrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CustomerCode = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FunctionTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Functions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    LobbySign = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BaseCharge = table.Column<double>(type: "REAL", nullable: false),
                    PerPersonCharge = table.Column<double>(type: "REAL", nullable: false),
                    GuaranteedNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SOCAN = table.Column<double>(type: "REAL", nullable: false),
                    Deposit = table.Column<double>(type: "REAL", nullable: false),
                    DepositPaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    NoHST = table.Column<bool>(type: "INTEGER", nullable: false),
                    NoGratuity = table.Column<bool>(type: "INTEGER", nullable: false),
                    CustomerID = table.Column<int>(type: "INTEGER", nullable: false),
                    FunctionTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Functions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Functions_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Functions_FunctionTypes_FunctionTypeID",
                        column: x => x.FunctionTypeID,
                        principalTable: "FunctionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerCode",
                table: "Customers",
                column: "CustomerCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Functions_CustomerID",
                table: "Functions",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Functions_FunctionTypeID",
                table: "Functions",
                column: "FunctionTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Functions");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "FunctionTypes");
        }
    }
}
