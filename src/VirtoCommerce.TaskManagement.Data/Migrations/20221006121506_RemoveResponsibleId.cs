using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.TaskManagement.Data.Migrations
{
    public partial class RemoveResponsibleId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkTask_ResponsibleId",
                table: "WorkTask");

            migrationBuilder.DropColumn(
                name: "ResponsibleId",
                table: "WorkTask");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_ResponsibleName",
                table: "WorkTask",
                column: "ResponsibleName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkTask_ResponsibleName",
                table: "WorkTask");

            migrationBuilder.AddColumn<string>(
                name: "ResponsibleId",
                table: "WorkTask",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_ResponsibleId",
                table: "WorkTask",
                column: "ResponsibleId");
        }
    }
}
