using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpecEdu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlppVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlppVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlppId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    Snapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeSummary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Source = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlppVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlppVersions_Plpps_PlppId",
                        column: x => x.PlppId,
                        principalTable: "Plpps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlppVersions_CreatedAt",
                table: "PlppVersions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PlppVersions_PlppId",
                table: "PlppVersions",
                column: "PlppId");

            migrationBuilder.CreateIndex(
                name: "IX_PlppVersions_PlppId_VersionNumber",
                table: "PlppVersions",
                columns: new[] { "PlppId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlppVersions_Source",
                table: "PlppVersions",
                column: "Source");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlppVersions");
        }
    }
}
