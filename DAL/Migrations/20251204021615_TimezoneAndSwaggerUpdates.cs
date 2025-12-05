using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class TimezoneAndSwaggerUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$biOxtTszFRDOiuS3x1wcTu.yXv9OJdRayiuF0fZCYZtHW46hNgzV." });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$69VbankiQm5Mhhgr9hkyr.8jvyzayoI3abiktAzdrnaV9isvN1qQe" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$ln0agCZNybwfCB/r3f9YlO.lVSPiSZkIyklmWKSefhIM4eOyCVmEO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$fwB9eUH7wP6jW7HOdvY0Be.ryJM1LD/inIUoma0CBiOleLVFIOJ.u" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$Rr6fwvG5lWY2qVa.D.CXx.Jq/bAA9gjWBE.uKq7JLURxCmBtN11oe" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$wgS5y9.YmohHF16vVY.H1ueTwyjDGSr3YtJyqeOz/HsOrkSpdc.Ry" });
        }
    }
}
