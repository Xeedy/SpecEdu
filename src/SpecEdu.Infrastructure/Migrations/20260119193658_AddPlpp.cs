using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpecEdu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlpp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plpps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchoolYear = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SupportLevel = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Strengths = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AreasNeedingSupport = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RecommendedMethods = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    OrganizationalAdjustments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ContentAdjustments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AssessmentMethods = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ParentCollaboration = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    InternalNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsVisibleToParents = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plpps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plpps_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlppEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlppId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluationMonth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WhatStudentManages = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    WhatNeedsImprovement = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RecommendedAdjustments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ParentConsultationNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ProgressRating = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ParentsNotified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ParentsNotifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlppEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlppEvaluations_Plpps_PlppId",
                        column: x => x.PlppId,
                        principalTable: "Plpps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlppGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlppId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SuccessCriteria = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Methods = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResponsiblePerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProgressNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlppGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlppGoals_Plpps_PlppId",
                        column: x => x.PlppId,
                        principalTable: "Plpps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlppEvaluations_EvaluationMonth",
                table: "PlppEvaluations",
                column: "EvaluationMonth");

            migrationBuilder.CreateIndex(
                name: "IX_PlppEvaluations_IsActive",
                table: "PlppEvaluations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PlppEvaluations_ParentsNotified",
                table: "PlppEvaluations",
                column: "ParentsNotified");

            migrationBuilder.CreateIndex(
                name: "IX_PlppEvaluations_PlppId",
                table: "PlppEvaluations",
                column: "PlppId");

            migrationBuilder.CreateIndex(
                name: "IX_PlppEvaluations_PlppId_EvaluationMonth",
                table: "PlppEvaluations",
                columns: new[] { "PlppId", "EvaluationMonth" });

            migrationBuilder.CreateIndex(
                name: "IX_PlppGoals_IsActive",
                table: "PlppGoals",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PlppGoals_PlppId",
                table: "PlppGoals",
                column: "PlppId");

            migrationBuilder.CreateIndex(
                name: "IX_PlppGoals_PlppId_Order",
                table: "PlppGoals",
                columns: new[] { "PlppId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_PlppGoals_PlppId_Status",
                table: "PlppGoals",
                columns: new[] { "PlppId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PlppGoals_Status",
                table: "PlppGoals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Plpps_IsActive",
                table: "Plpps",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Plpps_Status",
                table: "Plpps",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Plpps_StudentId",
                table: "Plpps",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Plpps_StudentId_SchoolYear",
                table: "Plpps",
                columns: new[] { "StudentId", "SchoolYear" });

            migrationBuilder.CreateIndex(
                name: "IX_Plpps_StudentId_Status",
                table: "Plpps",
                columns: new[] { "StudentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Plpps_ValidFrom_ValidTo",
                table: "Plpps",
                columns: new[] { "ValidFrom", "ValidTo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlppEvaluations");

            migrationBuilder.DropTable(
                name: "PlppGoals");

            migrationBuilder.DropTable(
                name: "Plpps");
        }
    }
}
