using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksApp.Migrations
{
    /// <inheritdoc />
    public partial class changingColsOfBooksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "books",
                newName: "Title");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "books",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "books");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "books",
                newName: "Name");
        }
    }
}
