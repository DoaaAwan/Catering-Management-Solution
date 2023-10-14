using Microsoft.EntityFrameworkCore.Migrations;

namespace CateringManagement.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetFunctionTimestampOnUpdate
                    AFTER UPDATE ON Functions
                    BEGIN
                        UPDATE Functions
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetFunctionTimestampOnInsert
                    AFTER INSERT ON Functions
                    BEGIN
                        UPDATE Functions
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        }
    }
}
