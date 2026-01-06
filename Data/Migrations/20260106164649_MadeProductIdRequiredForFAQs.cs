using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect_DAW_2025.Data.Migrations
{
    /// <inheritdoc />
    public partial class MadeProductIdRequiredForFAQs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FAQs_Products_ProductId",
                table: "FAQs");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "FAQs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FAQs_Products_ProductId",
                table: "FAQs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FAQs_Products_ProductId",
                table: "FAQs");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "FAQs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FAQs_Products_ProductId",
                table: "FAQs",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
