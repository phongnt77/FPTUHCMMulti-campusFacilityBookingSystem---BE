using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "campus",
                columns: new[] { "campus_id", "address", "created_at", "email", "facility_manager_id", "name", "phone_number", "updated_at" },
                values: new object[] { "C0001", "Lô E2a-7, Đường D1, Khu Công nghệ cao, P.Long Thạnh Mỹ, Tp. Thủ Đức, TP.HCM", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "daihocfpt@fpt.edu.vn", null, "FPTU HCM Campus", "028 7300 5588", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "facility_type",
                columns: new[] { "type_id", "created_at", "default_amenities", "default_capacity", "description", "icon_url", "name", "typical_duration_hours", "updated_at" },
                values: new object[,]
                {
                    { "FT0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", 40, "Standard classroom for lectures and seminars", "/icons/classroom.svg", "Classroom", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "FT0002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", 15, "Meeting room for group discussions", "/icons/meeting-room.svg", "Meeting Room", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "FT0003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"computers\": true, \"projector\": true, \"airConditioner\": true, \"printers\": true}", 30, "Computer laboratory with workstations", "/icons/computer-lab.svg", "Computer Lab", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "FT0004", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"scoreBoard\": true, \"lighting\": true, \"changeRoom\": true}", 20, "Sports court for basketball, volleyball, etc.", "/icons/sports-court.svg", "Sports Court", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "FT0005", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "{\"projector\": true, \"soundSystem\": true, \"stage\": true, \"airConditioner\": true}", 200, "Large auditorium for events and presentations", "/icons/auditorium.svg", "Auditorium", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "role_id",
                keyValue: "RL0001",
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "role_id",
                keyValue: "RL0002",
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "role_id",
                keyValue: "RL0003",
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "facility",
                columns: new[] { "facility_id", "amenities", "campus_id", "capacity", "created_at", "description", "facility_manager_id", "floor_number", "max_concurrent_bookings", "name", "room_number", "status", "type_id", "updated_at" },
                values: new object[,]
                {
                    { "F00001", "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true, \"speakers\": true}", "C0001", 40, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Standard classroom on first floor", null, "1", 1, "Room 101", "101", "Available", "FT0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00002", "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", "C0001", 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Small meeting room for group discussions", null, "2", 1, "Meeting Room A", "A201", "Available", "FT0002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00003", "{\"computers\": 30, \"projector\": true, \"airConditioner\": true, \"printers\": 2}", "C0001", 30, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Computer laboratory with 30 workstations", null, "3", 1, "Computer Lab 1", "LAB1", "Available", "FT0003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00004", "{\"scoreBoard\": true, \"lighting\": true, \"changeRoom\": true}", "C0001", 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Outdoor basketball court", null, "Ground", 1, "Basketball Court", "COURT-BB1", "Available", "FT0004", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00005", "{\"projector\": true, \"soundSystem\": true, \"stage\": true, \"airConditioner\": true, \"recording\": true}", "C0001", 200, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Large auditorium for events and ceremonies", null, "1", 1, "Main Auditorium", "AUD-MAIN", "Available", "FT0005", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "user_id", "avatar_url", "campus_id", "created_at", "email", "email_verification_token", "email_verification_token_expiry", "full_name", "last_login", "password", "phone_number", "role_id", "updated_at", "user_name" },
                values: new object[,]
                {
                    { "U00001", null, "C0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@fpt.edu.vn", null, null, "System Administrator", null, "$2a$11$2uBp1kZKohSrPSjXKJWkYuGgvBt9CL/98sLXxayo4L6f4qNq2N/yy", null, "RL0003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { "U00002", null, "C0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "lecturer@fpt.edu.vn", null, null, "John Lecturer", null, "$2a$11$IChyASPCgaMU.0.di/vY7.Z6ncK4hgs8tpV7YByEuTU3NybEAuppi", null, "RL0002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { "U00003", null, "C0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "student@fpt.edu.vn", null, null, "Jane Student", null, "$2a$11$duXOsfi1RzZaCz9iknV5/eRCi7WQtudrZrWRpgaFcY6AoVFyWhSOu", null, "RL0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00001");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00002");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00003");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00004");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00005");

            migrationBuilder.DeleteData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001");

            migrationBuilder.DeleteData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002");

            migrationBuilder.DeleteData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003");

            migrationBuilder.DeleteData(
                table: "campus",
                keyColumn: "campus_id",
                keyValue: "C0001");

            migrationBuilder.DeleteData(
                table: "facility_type",
                keyColumn: "type_id",
                keyValue: "FT0001");

            migrationBuilder.DeleteData(
                table: "facility_type",
                keyColumn: "type_id",
                keyValue: "FT0002");

            migrationBuilder.DeleteData(
                table: "facility_type",
                keyColumn: "type_id",
                keyValue: "FT0003");

            migrationBuilder.DeleteData(
                table: "facility_type",
                keyColumn: "type_id",
                keyValue: "FT0004");

            migrationBuilder.DeleteData(
                table: "facility_type",
                keyColumn: "type_id",
                keyValue: "FT0005");

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "role_id",
                keyValue: "RL0001",
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9951), new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9952) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "role_id",
                keyValue: "RL0002",
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9955), new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9955) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "role_id",
                keyValue: "RL0003",
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9957), new DateTime(2025, 12, 3, 2, 34, 11, 0, DateTimeKind.Utc).AddTicks(9958) });
        }
    }
}
