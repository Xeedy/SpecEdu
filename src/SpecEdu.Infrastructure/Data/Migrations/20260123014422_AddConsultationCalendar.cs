using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpecEdu.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConsultationCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsultationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OnlineMeetingLink = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsVisibleToParents = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AllowResponses = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PlppId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ReminderMinutesBefore = table.Column<int>(type: "int", nullable: true, defaultValue: 1440),
                    ReminderSent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationEvents_Plpps_PlppId",
                        column: x => x.PlppId,
                        principalTable: "Plpps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ConsultationEvents_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultationEvents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultationEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ExternalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExternalEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RoleDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResponseStatus = table.Column<int>(type: "int", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsOrganizer = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NotificationSent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NotificationSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Attended = table.Column<bool>(type: "bit", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationParticipants_ConsultationEvents_ConsultationEventId",
                        column: x => x.ConsultationEventId,
                        principalTable: "ConsultationEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationEvents_IsActive",
                table: "ConsultationEvents",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationEvents_PlppId",
                table: "ConsultationEvents",
                column: "PlppId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationEvents_ReminderSent_StartTime",
                table: "ConsultationEvents",
                columns: new[] { "ReminderSent", "StartTime" },
                filter: "[ReminderMinutesBefore] IS NOT NULL AND [ReminderSent] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationEvents_SchoolId",
                table: "ConsultationEvents",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationEvents_SchoolId_StartTime",
                table: "ConsultationEvents",
                columns: new[] { "SchoolId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationEvents_SchoolId_Status",
                table: "ConsultationEvents",
                columns: new[] { "SchoolId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationEvents_StartTime",
                table: "ConsultationEvents",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationEvents_Status",
                table: "ConsultationEvents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationEvents_StudentId",
                table: "ConsultationEvents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationEvents_Type",
                table: "ConsultationEvents",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationParticipants_ConsultationEventId",
                table: "ConsultationParticipants",
                column: "ConsultationEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationParticipants_ConsultationEventId_UserId",
                table: "ConsultationParticipants",
                columns: new[] { "ConsultationEventId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationParticipants_NotificationSent_ConsultationEventId",
                table: "ConsultationParticipants",
                columns: new[] { "NotificationSent", "ConsultationEventId" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationParticipants_ResponseStatus",
                table: "ConsultationParticipants",
                column: "ResponseStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationParticipants_UserId",
                table: "ConsultationParticipants",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultationParticipants");

            migrationBuilder.DropTable(
                name: "ConsultationEvents");
        }
    }
}
