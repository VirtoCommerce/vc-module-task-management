using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.TaskManagement.Data.Migrations
{
    public partial class AddObjectFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ObjectId",
                table: "WorkTask",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObjectType",
                table: "WorkTask",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_ObjectId",
                table: "WorkTask",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_ObjectType",
                table: "WorkTask",
                column: "ObjectType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkTask_ObjectId",
                table: "WorkTask");

            migrationBuilder.DropIndex(
                name: "IX_WorkTask_ObjectType",
                table: "WorkTask");

            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "WorkTask");

            migrationBuilder.DropColumn(
                name: "ObjectType",
                table: "WorkTask");
        }
    }
}
