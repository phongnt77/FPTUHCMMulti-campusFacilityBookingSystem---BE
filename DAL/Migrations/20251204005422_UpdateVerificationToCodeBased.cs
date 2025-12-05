using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVerificationToCodeBased : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email_verification_token",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "email_verification_token_expiry",
                table: "user",
                newName: "password_reset_code_expiry");

            migrationBuilder.AddColumn<string>(
                name: "email_verification_code",
                table: "user",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "email_verification_code_expiry",
                table: "user",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_reset_code",
                table: "user",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "email_verification_code", "email_verification_code_expiry", "is_verify", "password", "password_reset_code" },
                values: new object[] { null, null, "Unverified", "$2a$11$kEZDIjiuxh5xGevQigZ0a.QOk9040yBDRNgTldIxJUXrP4wTndoi6", null });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "email_verification_code", "email_verification_code_expiry", "is_verify", "password", "password_reset_code" },
                values: new object[] { null, null, "Unverified", "$2a$11$uRMsT5qJeUI1n9iHnro52etxwsrsntXtKqS.qt.lPfKL710L9Ut6y", null });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "email_verification_code", "email_verification_code_expiry", "is_verify", "password", "password_reset_code" },
                values: new object[] { null, null, "Unverified", "$2a$11$0ehSRhdkOGBWn.DHkpnS2Oho3gRtPPPIgLmBSZQw1EHWp/TmULJ/i", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email_verification_code",
                table: "user");

            migrationBuilder.DropColumn(
                name: "email_verification_code_expiry",
                table: "user");

            migrationBuilder.DropColumn(
                name: "password_reset_code",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "password_reset_code_expiry",
                table: "user",
                newName: "email_verification_token_expiry");

            migrationBuilder.AddColumn<string>(
                name: "email_verification_token",
                table: "user",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00001",
                columns: new[] { "email_verification_token", "is_verify", "password" },
                values: new object[] { null, "Verified", "$2a$11$/fnbmTnQfv85okwq6gjUru/WmqJoKxgB/Tf2EpgIhA7scqefvwCnq" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00002",
                columns: new[] { "email_verification_token", "is_verify", "password" },
                values: new object[] { null, "Verified", "$2a$11$6RVY2PaGRUSjAUra2qYdx.SO80hLw9dK6BmCZ4zND3Av.Beql/BKq" });

            migrationBuilder.UpdateData(
                table: "user",
                keyColumn: "user_id",
                keyValue: "U00003",
                columns: new[] { "email_verification_token", "is_verify", "password" },
                values: new object[] { null, "Verified", "$2a$11$wkaz/wwKFwuMtoO4BtOlFOAfnu.MvX0LrATgzg0.No4JLE76vUAOi" });
        }
    }
}
