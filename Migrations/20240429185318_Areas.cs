using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BooksApp.Migrations
{
    /// <inheritdoc />
    public partial class Areas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_books",
                table: "books");

            migrationBuilder.DropColumn(
                name: "DatePublished",
                table: "books");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "books");

            migrationBuilder.RenameTable(
                name: "books",
                newName: "Books");

            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                column: "BookID");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.ID);
                });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 1,
                columns: new[] { "CategoryID", "Description", "ImgUrl" },
                values: new object[] { 3, "Learn C#", "" });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 2,
                columns: new[] { "CategoryID", "ImgUrl", "Price" },
                values: new object[] { 3, "", 19.99m });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 3,
                columns: new[] { "CategoryID", "ImgUrl", "Price" },
                values: new object[] { 3, "", 19.99m });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "ID", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "How to Travel", "Travel" },
                    { 2, "Fictional Science", "Sci-Fi" },
                    { 3, "Technology", "Technology" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryID",
                table: "Books",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Books",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_CategoryID",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Books");

            migrationBuilder.RenameTable(
                name: "Books",
                newName: "books");

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePublished",
                table: "books",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_books",
                table: "books",
                column: "BookID");

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "BookID",
                keyValue: 1,
                columns: new[] { "DatePublished", "Description", "Genre" },
                values: new object[] { new DateTime(2023, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "This is useful", "Technology" });

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "BookID",
                keyValue: 2,
                columns: new[] { "DatePublished", "Genre", "Price" },
                values: new object[] { new DateTime(2023, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Technology", 15.99m });

            migrationBuilder.UpdateData(
                table: "books",
                keyColumn: "BookID",
                keyValue: 3,
                columns: new[] { "DatePublished", "Genre", "Price" },
                values: new object[] { new DateTime(2023, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Technology", 10.99m });
        }
    }
}
