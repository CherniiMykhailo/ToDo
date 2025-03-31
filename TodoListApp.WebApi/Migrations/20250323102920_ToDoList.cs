using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ToDoList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ToDoListId",
                table: "ToDos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ToDoLists",
                columns: table => new
                {
                    ToDoListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoLists", x => x.ToDoListId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDos_ToDoListId",
                table: "ToDos",
                column: "ToDoListId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDos_ToDoLists_ToDoListId",
                table: "ToDos",
                column: "ToDoListId",
                principalTable: "ToDoLists",
                principalColumn: "ToDoListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDos_ToDoLists_ToDoListId",
                table: "ToDos");

            migrationBuilder.DropTable(
                name: "ToDoLists");

            migrationBuilder.DropIndex(
                name: "IX_ToDos_ToDoListId",
                table: "ToDos");

            migrationBuilder.DropColumn(
                name: "ToDoListId",
                table: "ToDos");
        }
    }
}
