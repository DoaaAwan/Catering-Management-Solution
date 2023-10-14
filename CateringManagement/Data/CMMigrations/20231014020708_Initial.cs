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
                    CustomerCode = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
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
                name: "MealTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Functions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    LobbySign = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    SetupNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BaseCharge = table.Column<double>(type: "REAL", nullable: false),
                    PerPersonCharge = table.Column<double>(type: "REAL", nullable: false),
                    GuaranteedNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SOCAN = table.Column<double>(type: "REAL", nullable: false),
                    Deposit = table.Column<double>(type: "REAL", nullable: false),
                    DepositPaid = table.Column<bool>(type: "INTEGER", nullable: false),
                    NoHST = table.Column<bool>(type: "INTEGER", nullable: false),
                    NoGratuity = table.Column<bool>(type: "INTEGER", nullable: false),
                    Alcohol = table.Column<bool>(type: "INTEGER", nullable: false),
                    MealTypeID = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomerID = table.Column<int>(type: "INTEGER", nullable: false),
                    FunctionTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Functions_MealTypes_MealTypeID",
                        column: x => x.MealTypeID,
                        principalTable: "MealTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FunctionRooms",
                columns: table => new
                {
                    FunctionID = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionRooms", x => new { x.FunctionID, x.RoomID });
                    table.ForeignKey(
                        name: "FK_FunctionRooms_Functions_FunctionID",
                        column: x => x.FunctionID,
                        principalTable: "Functions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FunctionRooms_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerCode",
                table: "Customers",
                column: "CustomerCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FunctionRooms_RoomID",
                table: "FunctionRooms",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_Functions_CustomerID",
                table: "Functions",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Functions_FunctionTypeID",
                table: "Functions",
                column: "FunctionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Functions_MealTypeID",
                table: "Functions",
                column: "MealTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FunctionRooms");

            migrationBuilder.DropTable(
                name: "Functions");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "FunctionTypes");

            migrationBuilder.DropTable(
                name: "MealTypes");
        }
    }
}
