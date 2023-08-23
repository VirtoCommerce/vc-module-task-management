using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VirtoCommerce.TaskManagement.Data.PostgreSql.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkTask",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Number = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Group = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Priority = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ResponsibleId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ResponsibleName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    OrganizationId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Completed = table.Column<bool>(type: "boolean", nullable: true),
                    StoreId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    WorkflowId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Parameters = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<string>(type: "text", nullable: true),
                    ObjectId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ObjectType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkTaskAttachment",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Url = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Name = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    WorkTaskId = table.Column<string>(type: "character varying(128)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTaskAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkTaskAttachment_WorkTask_WorkTaskId",
                        column: x => x.WorkTaskId,
                        principalTable: "WorkTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_WorkTask_OrganizationId",
                table: "WorkTask",
                column: "OrganizationId");

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

            migrationBuilder.CreateIndex(
                name: "IX_WorkTaskAttachment_WorkTaskId",
                table: "WorkTaskAttachment",
                column: "WorkTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkTaskAttachment");

            migrationBuilder.DropTable(
                name: "WorkTask");
        }
    }
}
