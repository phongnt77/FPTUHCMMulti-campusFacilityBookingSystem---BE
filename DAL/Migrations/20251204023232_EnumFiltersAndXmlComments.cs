using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class EnumFiltersAndXmlComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$yUewVhi38UGe9Hv.pCe1b.PCg4hGj4TPi1zHG7QBlUazl06dN1mp." });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$LL1zVLOKE5ObIxp4VHb8guUI.nOJSpiT1uxS8thUrKpdA/YvsSOj." });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$Scj24gjHsoxFPCE7X6/eUuvNTZHdUNOjoumu3GeK8KfktIohCW8Dm" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$biOxtTszFRDOiuS3x1wcTu.yXv9OJdRayiuF0fZCYZtHW46hNgzV." });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$69VbankiQm5Mhhgr9hkyr.8jvyzayoI3abiktAzdrnaV9isvN1qQe" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$ln0agCZNybwfCB/r3f9YlO.lVSPiSZkIyklmWKSefhIM4eOyCVmEO" });
        }
    }
}
