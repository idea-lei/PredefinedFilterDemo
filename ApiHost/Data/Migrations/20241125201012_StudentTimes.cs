using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PredefinedFilterDemo.Data.Migrations
{
    /// <inheritdoc />
    public partial class StudentTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EntranceTime",
                table: "Students",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "GraduationTime",
                table: "Students",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntranceTime",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GraduationTime",
                table: "Students");
        }
    }
}
