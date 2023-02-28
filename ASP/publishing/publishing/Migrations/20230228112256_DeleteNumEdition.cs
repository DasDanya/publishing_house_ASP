using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace publishing.Migrations
{
    /// <inheritdoc />
    public partial class DeleteNumEdition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumEdition",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumEdition",
                table: "Products",
                type: "int",
                nullable: true);
        }
    }
}
