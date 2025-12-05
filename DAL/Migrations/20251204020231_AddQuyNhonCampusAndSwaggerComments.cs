using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddQuyNhonCampusAndSwaggerComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "campus",
                keyColumn: "campus_id",
                keyValue: "C0002",
                columns: new[] { "address", "email", "phone_number" },
                values: new object[] { "Khu đô thị Nhơn Hội, TP. Quy Nhơn, Bình Định", "qn@fpt.edu.vn", "0256 3842 468" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00001",
                columns: new[] { "description", "name" },
                values: new object[] { "Phòng học tiêu chuẩn tầng 1", "Phòng 101" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00002",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", 50, "Phòng học lớn tầng 1", "1", "Phòng 102", "102", "FT0001" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00003",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", 45, "Phòng học tầng 2", "2", "Phòng 201", "201", "FT0001" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00004",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", 15, "Phòng họp nhỏ", "2", "Phòng Họp A", "A201", "FT0002" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00005",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", 20, "Phòng họp vừa", "2", "Phòng Họp B", "B201", "FT0002" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00006",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"computers\": 30, \"projector\": true, \"airConditioner\": true, \"printers\": 2}", 30, "Phòng thực hành máy tính", "3", "Phòng Lab 1", "LAB1", "FT0003" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00007",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"computers\": 30, \"projector\": true, \"airConditioner\": true}", 30, "Phòng thực hành lập trình", "3", "Phòng Lab 2", "LAB2", "FT0003" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00008",
                columns: new[] { "amenities", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"scoreBoard\": true, \"lighting\": true, \"changeRoom\": true}", "Sân bóng rổ ngoài trời", "Ground", "Sân Bóng Rổ", "COURT-BB1", "FT0004" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00009",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"lighting\": true, \"changeRoom\": true}", 10, "Sân cầu lông trong nhà", "Ground", "Sân Cầu Lông", "COURT-BD1", "FT0004" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00010",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"soundSystem\": true, \"stage\": true, \"airConditioner\": true, \"recording\": true}", 200, "Hội trường lớn cho sự kiện", "1", "Hội Trường Chính", "AUD-MAIN", "FT0005" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00011",
                columns: new[] { "description", "name", "room_number" },
                values: new object[] { "Phòng học tầng 1 - QN", "Phòng QN-101", "QN-101" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00012",
                columns: new[] { "amenities", "capacity", "description", "name", "room_number" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", 35, "Phòng học tầng 1 - QN", "Phòng QN-102", "QN-102" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00013",
                columns: new[] { "amenities", "capacity", "description", "name", "room_number" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", 40, "Phòng học tầng 2 - QN", "Phòng QN-201", "QN-201" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00014",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", 45, "Phòng học tầng 2 - QN", "2", "Phòng QN-202", "QN-202", "FT0001" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00015",
                columns: new[] { "amenities", "capacity", "description", "name", "room_number" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", 12, "Phòng họp nhóm", "Phòng Họp QN-A", "QN-A301" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00016",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", 20, "Phòng họp lớn", "3", "Phòng Họp QN-B", "QN-B301", "FT0002" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00017",
                columns: new[] { "amenities", "capacity", "description", "name", "room_number" },
                values: new object[] { "{\"computers\": 35, \"projector\": true, \"airConditioner\": true, \"printers\": 2}", 35, "Phòng thực hành IT", "Lab QN-1", "QN-LAB1" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00018",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"computers\": 30, \"projector\": true, \"airConditioner\": true}", 30, "Phòng thực hành software", "4", "Lab QN-2", "QN-LAB2", "FT0003" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00019",
                columns: new[] { "amenities", "capacity", "description", "name", "room_number" },
                values: new object[] { "{\"lighting\": true, \"scoreBoard\": true}", 14, "Sân bóng đá mini ngoài trời", "Sân Bóng Đá Mini", "QN-COURT-F1" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00020",
                columns: new[] { "description", "name", "room_number" },
                values: new object[] { "Hội trường campus Quy Nhơn", "Hội Trường QN", "QN-AUD" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$fwB9eUH7wP6jW7HOdvY0Be.ryJM1LD/inIUoma0CBiOleLVFIOJ.u" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$Rr6fwvG5lWY2qVa.D.CXx.Jq/bAA9gjWBE.uKq7JLURxCmBtN11oe" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$wgS5y9.YmohHF16vVY.H1ueTwyjDGSr3YtJyqeOz/HsOrkSpdc.Ry" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "campus",
                keyColumn: "campus_id",
                keyValue: "C0002",
                columns: new[] { "address", "email", "phone_number" },
                values: new object[] { "Khu đô thị mới An Phú Thịnh, P.Nhơn Bình, TP.Quy Nhơn, Bình Định", "quynhon@fpt.edu.vn", "0256 3846 849" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00001",
                columns: new[] { "description", "name" },
                values: new object[] { "Standard classroom on first floor", "Room 101" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00002",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", 15, "Small meeting room for group discussions", "2", "Meeting Room A", "A201", "FT0002" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00003",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"computers\": 30, \"projector\": true, \"airConditioner\": true, \"printers\": 2}", 30, "Computer laboratory with 30 workstations", "3", "Computer Lab 1", "LAB1", "FT0003" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00004",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"scoreBoard\": true, \"lighting\": true, \"changeRoom\": true}", 20, "Outdoor basketball court", "Ground", "Basketball Court", "COURT-BB1", "FT0004" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00005",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"soundSystem\": true, \"stage\": true, \"airConditioner\": true, \"recording\": true}", 200, "Large auditorium for events and ceremonies", "1", "Main Auditorium", "AUD-MAIN", "FT0005" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00006",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", 45, "Standard classroom", "1", "Room 102", "102", "FT0001" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00007",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true}", 50, "Large classroom", "2", "Room 201", "201", "FT0001" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00008",
                columns: new[] { "amenities", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", "Medium meeting room", "2", "Meeting Room B", "B202", "FT0002" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00009",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"computers\": 35, \"projector\": true, \"airConditioner\": true, \"printers\": 2}", 35, "Computer lab with software development tools", "3", "Computer Lab 2", "LAB2", "FT0003" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00010",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"scoreBoard\": true, \"lighting\": true, \"changeRoom\": true}", 20, "Indoor volleyball court", "Ground", "Volleyball Court", "COURT-VB1", "FT0004" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00011",
                columns: new[] { "description", "name", "room_number" },
                values: new object[] { "Standard classroom", "Room QN-101", "101" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00012",
                columns: new[] { "amenities", "capacity", "description", "name", "room_number" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true, \"speakers\": true}", 50, "Large classroom", "Room QN-102", "102" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00013",
                columns: new[] { "amenities", "capacity", "description", "name", "room_number" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"airConditioner\": true, \"smartTV\": true}", 45, "Multimedia classroom", "Room QN-201", "201" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00014",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true, \"videoConference\": true}", 15, "Meeting room for group discussions", "3", "Meeting Room QN-A", "A301", "FT0002" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00015",
                columns: new[] { "amenities", "capacity", "description", "name", "room_number" },
                values: new object[] { "{\"projector\": true, \"whiteboard\": true}", 10, "Small meeting room", "Meeting Room QN-B", "B301" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00016",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"computers\": 30, \"projector\": true, \"airConditioner\": true, \"printers\": 2}", 30, "Modern computer laboratory", "4", "Computer Lab QN-1", "LAB1", "FT0003" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00017",
                columns: new[] { "amenities", "capacity", "description", "name", "room_number" },
                values: new object[] { "{\"computers\": 28, \"projector\": true, \"airConditioner\": true, \"graphicsTablets\": 5}", 28, "Computer lab with design software", "Computer Lab QN-2", "LAB2" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00018",
                columns: new[] { "amenities", "capacity", "description", "floor_number", "name", "room_number", "type_id" },
                values: new object[] { "{\"scoreBoard\": true, \"lighting\": true, \"changeRoom\": true}", 20, "Outdoor basketball court", "Ground", "Basketball Court QN", "COURT-BB1", "FT0004" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00019",
                columns: new[] { "amenities", "capacity", "description", "name", "room_number" },
                values: new object[] { "{\"scoreBoard\": true, \"lighting\": true, \"changeRoom\": true}", 16, "Indoor badminton court", "Badminton Court QN", "COURT-BD1" });

            migrationBuilder.UpdateData(
                table: "facility",
                keyColumn: "facility_id",
                keyValue: "F00020",
                columns: new[] { "description", "name", "room_number" },
                values: new object[] { "Main auditorium for events", "Auditorium QN", "AUD-QN" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$RTYYMnbA4u4AsFjrJ0icJuGFK2HIQz2Bvm3DsGuIgSuVQ0f8Dkqxq" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$VF3bBOD5oWzoYJ9q1b621eczagjaTb9kTVqGwcKM3XSplZzMV8jTe" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$OVIelNQuFNCk1yUZyNjEsOX4IVLLVlYE.L6nezbcnCCGWrTSi.XDS" });
        }
    }
}
