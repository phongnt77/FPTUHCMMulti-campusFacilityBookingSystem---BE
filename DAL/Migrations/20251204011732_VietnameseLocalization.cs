using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class VietnameseLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$TIVU8ZE3OrfuOviEr6tsX.9eU.7TRCXyvyRmlox1ZRvOew0spEt4." });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$UJSb0psh.BKKtXpTARFv9OR.x1o0rBV.7bKLQWdr4fQLCgPXw2x6e" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$M4UloZrPQdux69a/l0l4S.PM/rgqw2r0xdSD1URMNLbvEx/V4gkWC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$kEZDIjiuxh5xGevQigZ0a.QOk9040yBDRNgTldIxJUXrP4wTndoi6" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$uRMsT5qJeUI1n9iHnro52etxwsrsntXtKqS.qt.lPfKL710L9Ut6y" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$0ehSRhdkOGBWn.DHkpnS2Oho3gRtPPPIgLmBSZQw1EHWp/TmULJ/i" });
        }
    }
}
