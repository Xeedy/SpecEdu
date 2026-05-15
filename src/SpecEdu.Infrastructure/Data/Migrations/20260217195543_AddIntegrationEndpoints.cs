using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpecEdu.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIntegrationEndpoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntegrationEndpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SystemType = table.Column<int>(type: "int", nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApiKeyPlaceholder = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastTestedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastTestResult = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegrationEndpoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataExchangeRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndpointId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResponseSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InitiatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataExchangeRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataExchangeRecords_IntegrationEndpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "IntegrationEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeRecords_CreatedAt",
                table: "DataExchangeRecords",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeRecords_EndpointId",
                table: "DataExchangeRecords",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeRecords_EndpointId_CreatedAt",
                table: "DataExchangeRecords",
                columns: new[] { "EndpointId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeRecords_EndpointId_Status",
                table: "DataExchangeRecords",
                columns: new[] { "EndpointId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_DataExchangeRecords_Status_NonTerminal",
                table: "DataExchangeRecords",
                column: "Status",
                filter: "[Status] IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEndpoints_IsActive",
                table: "IntegrationEndpoints",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEndpoints_SystemType",
                table: "IntegrationEndpoints",
                column: "SystemType");

            migrationBuilder.CreateIndex(
                name: "IX_IntegrationEndpoints_SystemType_IsActive",
                table: "IntegrationEndpoints",
                columns: new[] { "SystemType", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataExchangeRecords");

            migrationBuilder.DropTable(
                name: "IntegrationEndpoints");
        }
    }
}
