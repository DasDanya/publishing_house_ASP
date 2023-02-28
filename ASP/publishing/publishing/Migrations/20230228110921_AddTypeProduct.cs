using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace publishing.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Margin",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "TypeProductId",
                table: "Products",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TypeProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Margin = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeProducts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_TypeProductId",
                table: "Products",
                column: "TypeProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_TypeProducts_TypeProductId",
                table: "Products",
                column: "TypeProductId",
                principalTable: "TypeProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_TypeProducts_TypeProductId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "TypeProducts");

            migrationBuilder.DropIndex(
                name: "IX_Products_TypeProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TypeProductId",
                table: "Products");

            migrationBuilder.AddColumn<double>(
                name: "Margin",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Products",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }
    }
}
