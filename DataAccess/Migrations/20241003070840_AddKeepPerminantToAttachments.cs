using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddKeepPerminantToAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "published",
                table: "Assignments",
                newName: "Published");

            migrationBuilder.AddColumn<bool>(
                name: "KeepPerminant",
                table: "AssignmentAttachments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeepPerminant",
                table: "AssignmentAttachments");

            migrationBuilder.RenameColumn(
                name: "Published",
                table: "Assignments",
                newName: "published");
        }
    }
}
