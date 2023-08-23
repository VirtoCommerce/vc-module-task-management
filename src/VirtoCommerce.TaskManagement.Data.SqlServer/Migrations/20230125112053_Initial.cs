using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.TaskManagement.Data.SqlServer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkTask",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Number = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Group = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ResponsibleId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ResponsibleName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: true),
                    StoreId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    WorkflowId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObjectId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ObjectType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTask", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_Completed",
                table: "WorkTask",
                column: "Completed");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_DueDate",
                table: "WorkTask",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_IsActive",
                table: "WorkTask",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_Number",
                table: "WorkTask",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_ObjectId",
                table: "WorkTask",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_ObjectType",
                table: "WorkTask",
                column: "ObjectType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_ResponsibleId",
                table: "WorkTask",
                column: "ResponsibleId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_Status",
                table: "WorkTask",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTask_StoreId",
                table: "WorkTask",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkTask");
        }
    }
}
