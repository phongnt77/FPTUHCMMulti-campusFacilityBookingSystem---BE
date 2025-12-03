using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class RestoreUserIdInBookingAndRemoveBookingUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "booking_user");

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "booking",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$s6tzBm5UOEARue3nddBm0eAb1/o6PE3fcr3WrfmVgakpha3J0QPHK" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$owxfU2RkpStLv7FelhmaNuGiLbTckUPJQ58fXKGWbYxwiRtBbZ25S" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$kpvtQ60SxnkEzWcJJOOyf.e4C3wQnm54uA4IdeWdGMcKXtprID5iW" });

            migrationBuilder.CreateIndex(
                name: "IX_booking_user_id",
                table: "booking",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_booking_user_user_id",
                table: "booking",
                column: "user_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_booking_user_user_id",
                table: "booking");

            migrationBuilder.DropIndex(
                name: "IX_booking_user_id",
                table: "booking");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "booking");

            migrationBuilder.CreateTable(
                name: "booking_user",
                columns: table => new
                {
                    booking_user_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    booking_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    relation_type = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: "Participant")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_user", x => x.booking_user_id);
                    table.ForeignKey(
                        name: "FK_booking_user_booking_booking_id",
                        column: x => x.booking_id,
                        principalTable: "booking",
                        principalColumn: "booking_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_booking_user_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$2uBp1kZKohSrPSjXKJWkYuGgvBt9CL/98sLXxayo4L6f4qNq2N/yy" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$IChyASPCgaMU.0.di/vY7.Z6ncK4hgs8tpV7YByEuTU3NybEAuppi" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$duXOsfi1RzZaCz9iknV5/eRCi7WQtudrZrWRpgaFcY6AoVFyWhSOu" });

            migrationBuilder.CreateIndex(
                name: "IX_booking_user_booking_id_user_id_relation_type",
                table: "booking_user",
                columns: new[] { "booking_id", "user_id", "relation_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_booking_user_user_id",
                table: "booking_user",
                column: "user_id");
        }
    }
}
