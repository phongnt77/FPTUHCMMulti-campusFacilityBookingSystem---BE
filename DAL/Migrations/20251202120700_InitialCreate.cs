using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "facility_type",
                columns: table => new
                {
                    type_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    default_amenities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    default_capacity = table.Column<int>(type: "int", nullable: true),
                    typical_duration_hours = table.Column<int>(type: "int", nullable: true),
                    icon_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facility_type", x => x.type_id);
                });

            migrationBuilder.CreateTable(
                name: "booking",
                columns: table => new
                {
                    booking_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    facility_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    purpose = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    estimated_attendees = table.Column<int>(type: "int", nullable: true),
                    special_requirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: "Draft"),
                    approved_by = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    rejection_reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    check_in_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    check_out_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_used = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    cancelled_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    cancellation_reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking", x => x.booking_id);
                });

            migrationBuilder.CreateTable(
                name: "booking_feedback",
                columns: table => new
                {
                    feedback_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    booking_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comments = table.Column<string>(type: "text", nullable: true),
                    report_issue = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    issue_description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    resolved_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_feedback", x => x.feedback_id);
                    table.ForeignKey(
                        name: "FK_booking_feedback_booking_booking_id",
                        column: x => x.booking_id,
                        principalTable: "booking",
                        principalColumn: "booking_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "booking_history",
                columns: table => new
                {
                    history_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    booking_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    modified_by = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    field_changed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    old_value = table.Column<string>(type: "text", nullable: true),
                    new_value = table.Column<string>(type: "text", nullable: true),
                    reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    changed_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_history", x => x.history_id);
                    table.ForeignKey(
                        name: "FK_booking_history_booking_booking_id",
                        column: x => x.booking_id,
                        principalTable: "booking",
                        principalColumn: "booking_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campus",
                columns: table => new
                {
                    campus_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    phone_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    facility_manager_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    status = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: "Active"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campus", x => x.campus_id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    user_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    role = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    campus_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    status = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: "Active"),
                    is_verify = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Unverified"),
                    avatar_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    last_login = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_user_campus_campus_id",
                        column: x => x.campus_id,
                        principalTable: "campus",
                        principalColumn: "campus_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "facility",
                columns: table => new
                {
                    facility_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    capacity = table.Column<int>(type: "int", nullable: false),
                    room_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    floor_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    campus_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    type_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    amenities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    facility_manager_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    max_concurrent_bookings = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facility", x => x.facility_id);
                    table.ForeignKey(
                        name: "FK_facility_campus_campus_id",
                        column: x => x.campus_id,
                        principalTable: "campus",
                        principalColumn: "campus_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_facility_facility_type_type_id",
                        column: x => x.type_id,
                        principalTable: "facility_type",
                        principalColumn: "type_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_facility_user_facility_manager_id",
                        column: x => x.facility_manager_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    notification_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    message = table.Column<string>(type: "text", nullable: true),
                    related_entity_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    related_entity_id = table.Column<int>(type: "int", nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    read_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_notification_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_images",
                columns: table => new
                {
                    image_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    facility_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    upload_date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    image_order = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facility_images", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_facility_images_facility_facility_id",
                        column: x => x.facility_id,
                        principalTable: "facility",
                        principalColumn: "facility_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "facility_maintenance",
                columns: table => new
                {
                    maintenance_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    facility_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    issue_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    priority = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Medium"),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    assigned_to = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    scheduled_start = table.Column<DateTime>(type: "datetime2", nullable: true),
                    scheduled_end = table.Column<DateTime>(type: "datetime2", nullable: true),
                    actual_start = table.Column<DateTime>(type: "datetime2", nullable: true),
                    actual_end = table.Column<DateTime>(type: "datetime2", nullable: true),
                    completion_notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facility_maintenance", x => x.maintenance_id);
                    table.ForeignKey(
                        name: "FK_facility_maintenance_facility_facility_id",
                        column: x => x.facility_id,
                        principalTable: "facility",
                        principalColumn: "facility_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_facility_maintenance_user_assigned_to",
                        column: x => x.assigned_to,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "report",
                columns: table => new
                {
                    report_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    report_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    report_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    campus_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    facility_id = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    report_period_start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    report_period_end = table.Column<DateTime>(type: "datetime2", nullable: false),
                    period_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    total_bookings = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    approved_bookings = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    rejected_bookings = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    completed_bookings = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    utilization_rate = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    avg_rating = table.Column<decimal>(type: "decimal(3,2)", nullable: false, defaultValue: 0m),
                    unique_users = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    peak_hours = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    busiest_day = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    generated_by = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    generated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    is_published = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report", x => x.report_id);
                    table.ForeignKey(
                        name: "FK_report_campus_campus_id",
                        column: x => x.campus_id,
                        principalTable: "campus",
                        principalColumn: "campus_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_report_facility_facility_id",
                        column: x => x.facility_id,
                        principalTable: "facility",
                        principalColumn: "facility_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_report_user_generated_by",
                        column: x => x.generated_by,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_booking_approved_by",
                table: "booking",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_booking_facility_id_start_time_end_time_status",
                table: "booking",
                columns: new[] { "facility_id", "start_time", "end_time", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_booking_user_id",
                table: "booking",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_feedback_booking_id",
                table: "booking_feedback",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_feedback_user_id",
                table: "booking_feedback",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_history_booking_id",
                table: "booking_history",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_history_modified_by",
                table: "booking_history",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "IX_campus_facility_manager_id",
                table: "campus",
                column: "facility_manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_campus_name",
                table: "campus",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_campus_status",
                table: "campus",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_facility_campus_id",
                table: "facility",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_facility_campus_id_name",
                table: "facility",
                columns: new[] { "campus_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_facility_facility_manager_id",
                table: "facility",
                column: "facility_manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_facility_name",
                table: "facility",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_facility_status",
                table: "facility",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_facility_type_id",
                table: "facility",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_facility_images_facility_id",
                table: "facility_images",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "IX_facility_maintenance_assigned_to",
                table: "facility_maintenance",
                column: "assigned_to");

            migrationBuilder.CreateIndex(
                name: "IX_facility_maintenance_facility_id",
                table: "facility_maintenance",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "IX_facility_type_name",
                table: "facility_type",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_created_at",
                table: "notification",
                column: "created_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_notification_is_read",
                table: "notification",
                column: "is_read");

            migrationBuilder.CreateIndex(
                name: "IX_notification_user_id",
                table: "notification",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_campus_id",
                table: "report",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_facility_id",
                table: "report",
                column: "facility_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_generated_by",
                table: "report",
                column: "generated_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_campus_id",
                table: "user",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_role",
                table: "user",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "IX_user_status",
                table: "user",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_user_user_name",
                table: "user",
                column: "user_name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_facility_facility_id",
                table: "booking",
                column: "facility_id",
                principalTable: "facility",
                principalColumn: "facility_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_user_approved_by",
                table: "booking",
                column: "approved_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_user_user_id",
                table: "booking",
                column: "user_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_feedback_user_user_id",
                table: "booking_feedback",
                column: "user_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_booking_history_user_modified_by",
                table: "booking_history",
                column: "modified_by",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_campus_user_facility_manager_id",
                table: "campus",
                column: "facility_manager_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campus_user_facility_manager_id",
                table: "campus");

            migrationBuilder.DropTable(
                name: "booking_feedback");

            migrationBuilder.DropTable(
                name: "booking_history");

            migrationBuilder.DropTable(
                name: "facility_images");

            migrationBuilder.DropTable(
                name: "facility_maintenance");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "report");

            migrationBuilder.DropTable(
                name: "booking");

            migrationBuilder.DropTable(
                name: "facility");

            migrationBuilder.DropTable(
                name: "facility_type");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "campus");
        }
    }
}
