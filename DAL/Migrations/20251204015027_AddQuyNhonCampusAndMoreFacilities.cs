using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddQuyNhonCampusAndMoreFacilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "campus",
                columns: new[] { "campus_id", "address", "created_at", "email", "facility_manager_id", "name", "phone_number", "updated_at" },
                values: new object[] { "C0002", "Khu đô thị mới An Phú Thịnh, P.Nhơn Bình, TP.Quy Nhơn, Bình Định", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "quynhon@fpt.edu.vn", null, "FPTU Quy Nhơn Campus", "0256 3846 849", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "facility",
                columns: new[] { "facility_id", "amenities", "campus_id", "capacity", "created_at", "description", "facility_manager_id", "floor_number", "max_concurrent_bookings", "name", "room_number", "status", "type_id", "updated_at" },
                values: new object[,]
                {
                    { "F00006", "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", "C0001", 45, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Standard classroom", null, "1", 1, "Room 102", "102", "Available", "FT0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00007", "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", "C0001", 50, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Large classroom", null, "2", 1, "Room 201", "201", "Available", "FT0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00008", "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", "C0001", 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Medium meeting room", null, "2", 1, "Meeting Room B", "B202", "Available", "FT0002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00009", "{\"computers\": 35, \"projector\": true, \"airConditioner\": true, \"printers\": 2}", "C0001", 35, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Computer lab with software development tools", null, "3", 1, "Computer Lab 2", "LAB2", "Available", "FT0003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00010", "{\"scoreBoard\": true, \"lighting\": true, \"changeRoom\": true}", "C0001", 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Indoor volleyball court", null, "Ground", 1, "Volleyball Court", "COURT-VB1", "Available", "FT0004", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$RTYYMnbA4u4AsFjrJ0icJuGFK2HIQz2Bvm3DsGuIgSuVQ0f8Dkqxq" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$VF3bBOD5oWzoYJ9q1b621eczagjaTb9kTVqGwcKM3XSplZzMV8jTe" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$OVIelNQuFNCk1yUZyNjEsOX4IVLLVlYE.L6nezbcnCCGWrTSi.XDS" });

            migrationBuilder.InsertData(
                table: "facility",
                columns: new[] { "facility_id", "amenities", "campus_id", "capacity", "created_at", "description", "facility_manager_id", "floor_number", "max_concurrent_bookings", "name", "room_number", "status", "type_id", "updated_at" },
                values: new object[,]
                {
                    { "F00011", "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", "C0002", 40, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Standard classroom", null, "1", 1, "Room QN-101", "101", "Available", "FT0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00012", "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true, \"speakers\": true}", "C0002", 50, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Large classroom", null, "1", 1, "Room QN-102", "102", "Available", "FT0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00013", "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true, \"smartTV\": true}", "C0002", 45, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Multimedia classroom", null, "2", 1, "Room QN-201", "201", "Available", "FT0001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00014", "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", "C0002", 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Meeting room for group discussions", null, "3", 1, "Meeting Room QN-A", "A301", "Available", "FT0002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00015", "{\"projector\": true, \"whiteboard\": true}", "C0002", 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Small meeting room", null, "3", 1, "Meeting Room QN-B", "B301", "Available", "FT0002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00016", "{\"computers\": 30, \"projector\": true, \"airConditioner\": true, \"printers\": 2}", "C0002", 30, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Modern computer laboratory", null, "4", 1, "Computer Lab QN-1", "LAB1", "Available", "FT0003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00017", "{\"computers\": 28, \"projector\": true, \"airConditioner\": true, \"graphicsTablets\": 5}", "C0002", 28, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Computer lab with design software", null, "4", 1, "Computer Lab QN-2", "LAB2", "Available", "FT0003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00018", "{\"scoreBoard\": true, \"lighting\": true, \"changeRoom\": true}", "C0002", 20, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Outdoor basketball court", null, "Ground", 1, "Basketball Court QN", "COURT-BB1", "Available", "FT0004", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00019", "{\"scoreBoard\": true, \"lighting\": true, \"changeRoom\": true}", "C0002", 16, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Indoor badminton court", null, "Ground", 1, "Badminton Court QN", "COURT-BD1", "Available", "FT0004", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { "F00020", "{\"projector\": true, \"soundSystem\": true, \"stage\": true, \"airConditioner\": true}", "C0002", 150, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Main auditorium for events", null, "1", 1, "Auditorium QN", "AUD-QN", "Available", "FT0005", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00006");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00007");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00008");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00009");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00010");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00011");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00012");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00013");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00014");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00015");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00016");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00017");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00018");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00019");

            migrationBuilder.DeleteData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00020");

            migrationBuilder.DeleteData(
                table: "campus",
                keyColumn: "campus_id",
                keyValue: "C0002");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$TIVU8ZE3OrfuOviEr6tsX.9eU.7TRCXyvyRmlox1ZRvOew0spEt4." });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$UJSb0psh.BKKtXpTARFv9OR.x1o0rBV.7bKLQWdr4fQLCgPXw2x6e" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$M4UloZrPQdux69a/l0l4S.PM/rgqw2r0xdSD1URMNLbvEx/V4gkWC" });
        }
    }
}
