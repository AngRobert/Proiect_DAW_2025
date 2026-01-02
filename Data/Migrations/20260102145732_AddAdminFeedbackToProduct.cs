using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect_DAW_2025.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminFeedbackToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminFeedback",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminFeedback",
                table: "Products");
        }
    }
}
