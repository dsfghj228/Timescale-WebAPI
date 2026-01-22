using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Values_FileImportId_ExecutionTime",
                table: "Values");

            migrationBuilder.CreateIndex(
                name: "IX_Values_FileImportId_Timestamp",
                table: "Values",
                columns: new[] { "FileImportId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Values_FileImportId_Timestamp",
                table: "Values");

            migrationBuilder.CreateIndex(
                name: "IX_Values_FileImportId_ExecutionTime",
                table: "Values",
                columns: new[] { "FileImportId", "ExecutionTime" });
        }
    }
}
