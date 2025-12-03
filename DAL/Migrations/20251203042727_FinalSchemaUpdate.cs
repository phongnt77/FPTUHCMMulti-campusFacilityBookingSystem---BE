using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class FinalSchemaUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$/fnbmTnQfv85okwq6gjUru/WmqJoKxgB/Tf2EpgIhA7scqefvwCnq" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$6RVY2PaGRUSjAUra2qYdx.SO80hLw9dK6BmCZ4zND3Av.Beql/BKq" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Unverified", "$2a$11$wkaz/wwKFwuMtoO4BtOlFOAfnu.MvX0LrATgzg0.No4JLE76vUAOi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$s6tzBm5UOEARue3nddBm0eAb1/o6PE3fcr3WrfmVgakpha3J0QPHK" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$owxfU2RkpStLv7FelhmaNuGiLbTckUPJQ58fXKGWbYxwiRtBbZ25S" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "is_verify", "password" },
                values: new object[] { "Verified", "$2a$11$kpvtQ60SxnkEzWcJJOOyf.e4C3wQnm54uA4IdeWdGMcKXtprID5iW" });
        }
    }
}
