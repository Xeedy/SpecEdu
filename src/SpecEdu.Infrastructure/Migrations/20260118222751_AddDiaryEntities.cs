using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpecEdu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiaryEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiaryEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Visibility = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiaryEntries_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiaryAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiaryEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiaryAttachments_DiaryEntries_DiaryEntryId",
                        column: x => x.DiaryEntryId,
                        principalTable: "DiaryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiaryAttachments_DiaryEntryId",
                table: "DiaryAttachments",
                column: "DiaryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEntries_CreatedBy",
                table: "DiaryEntries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEntries_IsActive",
                table: "DiaryEntries",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEntries_StudentId",
                table: "DiaryEntries",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEntries_StudentId_CreatedAt",
                table: "DiaryEntries",
                columns: new[] { "StudentId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEntries_StudentId_Type",
                table: "DiaryEntries",
                columns: new[] { "StudentId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEntries_StudentId_Visibility",
                table: "DiaryEntries",
                columns: new[] { "StudentId", "Visibility" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiaryAttachments");

            migrationBuilder.DropTable(
                name: "DiaryEntries");
        }
    }
}
