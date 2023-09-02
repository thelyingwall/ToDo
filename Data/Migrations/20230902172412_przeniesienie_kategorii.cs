using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo.Data.Migrations
{
    /// <inheritdoc />
    public partial class przeniesienie_kategorii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_Category_CategoryId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_CategoryId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Task");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "TaskList",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TaskList_CategoryId",
                table: "TaskList",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskList_Category_CategoryId",
                table: "TaskList",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskList_Category_CategoryId",
                table: "TaskList");

            migrationBuilder.DropIndex(
                name: "IX_TaskList_CategoryId",
                table: "TaskList");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "TaskList");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Task",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Task_CategoryId",
                table: "Task",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Category_CategoryId",
                table: "Task",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
