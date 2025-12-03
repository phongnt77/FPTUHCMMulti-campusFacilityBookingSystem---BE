using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyRoleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_role_role_code",
                table: "role");

            migrationBuilder.DropColumn(
                name: "description",
                table: "role");

            migrationBuilder.DropColumn(
                name: "permissions",
                table: "role");

            migrationBuilder.DropColumn(
                name: "role_code",
                table: "role");

            migrationBuilder.DropColumn(
                name: "status",
                table: "role");

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "role_id", "created_at", "role_name", "updated_at" },
                values: new object[,]
                {
                    { "RL0001", new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9951), "Student", new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9952) },
                    { "RL0002", new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9955), "Lecturer", new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9955) },
                    { "RL0003", new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9957), "Facility_Admin", new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9958) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_role_role_name",
                table: "role",
                column: "role_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_role_role_name",
                table: "role");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "role_id",
                keyValue: "RL0001");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "role_id",
                keyValue: "RL0002");

            migrationBuilder.DeleteData(
                table: "role",
                keyColumn: "role_id",
                keyValue: "RL0003");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "role",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "permissions",
                table: "role",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "role_code",
                table: "role",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "role",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_role_role_code",
                table: "role",
                column: "role_code",
                unique: true);
        }
    }
}
