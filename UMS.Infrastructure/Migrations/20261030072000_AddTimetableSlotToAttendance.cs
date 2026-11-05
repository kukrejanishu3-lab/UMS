using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTimetableSlotToAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_SectionId_StudentId_Date",
                table: "Attendances");

            migrationBuilder.AddColumn<Guid>(
                name: "TimetableId",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SectionId_StudentId_Date_TimetableId",
                table: "Attendances",
                columns: new[] { "SectionId", "StudentId", "Date", "TimetableId" },
                unique: true,
                filter: "[TimetableId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_TimetableId",
                table: "Attendances",
                column: "TimetableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Timetables_TimetableId",
                table: "Attendances",
                column: "TimetableId",
                principalTable: "Timetables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Timetables_TimetableId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_SectionId_StudentId_Date_TimetableId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_TimetableId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "TimetableId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_SectionId_StudentId_Date",
                table: "Attendances",
                columns: new[] { "SectionId", "StudentId", "Date" },
                unique: true);
        }
    }
}
