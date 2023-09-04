using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo.Data.Migrations
{
    /// <inheritdoc />
    public partial class nowa_proba_dodania_usera_po_zawale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TaskList",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskList_UserId",
                table: "TaskList",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskList_AspNetUsers_UserId",
                table: "TaskList",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskList_AspNetUsers_UserId",
                table: "TaskList");

            migrationBuilder.DropIndex(
                name: "IX_TaskList_UserId",
                table: "TaskList");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskList");
        }
    }
}
