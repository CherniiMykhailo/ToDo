using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedToField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "ToDos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "ToDos");
        }
    }
}
