using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstAidPlus.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "courses",
                newName: "category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "category",
                table: "courses",
                newName: "Category");
        }
    }
}
