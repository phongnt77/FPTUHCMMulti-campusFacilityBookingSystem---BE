using DAL.Models;
using DAL.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace DAL.Dbcontext
{
    public class FacilityBookingDbContext : DbContext
    {
        public FacilityBookingDbContext(DbContextOptions<FacilityBookingDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<Campus> Campuses { get; set; }
        public DbSet<FacilityType> FacilityTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<FacilityImage> FacilityImages { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingHistory> BookingHistories { get; set; }
        public DbSet<FacilityMaintenance> FacilityMaintenances { get; set; }
        public DbSet<BookingFeedback> BookingFeedbacks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Campus configuration
            modelBuilder.Entity<Campus>(entity =>
            {
                entity.ToTable("campus");
                entity.HasKey(e => e.CampusId);
                entity.Property(e => e.CampusId)
                    .HasColumnName("campus_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(255);
                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("phone_number")
                    .HasMaxLength(20);
                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100);
                entity.Property(e => e.FacilityManagerId)
                    .HasColumnName("facility_manager_id")
                    .HasMaxLength(6);
                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion<string>()
                    .HasDefaultValue(CampusStatus.Active);
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                // Unique constraint
                entity.HasIndex(e => e.Name).IsUnique();
                // Indexes
                entity.HasIndex(e => e.Status);

                // Relationship: Campus -> FacilityManager (User)
                entity.HasOne(e => e.FacilityManager)
                    .WithMany(u => u.ManagedCampuses)
                    .HasForeignKey(e => e.FacilityManagerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // FacilityType configuration
            modelBuilder.Entity<FacilityType>(entity =>
            {
                entity.ToTable("facility_type");
                entity.HasKey(e => e.TypeId);
                entity.Property(e => e.TypeId)
                    .HasColumnName("type_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");
                entity.Property(e => e.DefaultAmenities)
                    .HasColumnName("default_amenities")
                    .HasColumnType("nvarchar(max)"); // JSON
                entity.Property(e => e.DefaultCapacity)
                    .HasColumnName("default_capacity");
                entity.Property(e => e.TypicalDurationHours)
                    .HasColumnName("typical_duration_hours");
                entity.Property(e => e.IconUrl)
                    .HasColumnName("icon_url")
                    .HasMaxLength(255);
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                // Unique constraint
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.FullName)
                    .HasColumnName("full_name")
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("phone_number")
                    .HasMaxLength(20);
                entity.Property(e => e.UserName)
                    .HasColumnName("user_name")
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(255);
                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(e => e.CampusId)
                    .HasColumnName("campus_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion<string>()
                    .HasDefaultValue(UserStatus.Active);
                entity.Property(e => e.IsVerify)
                    .HasColumnName("is_verify")
                    .HasConversion<string>()
                    .HasDefaultValue(VerificationStatus.Unverified);
                entity.Property(e => e.AvatarUrl)
                    .HasColumnName("avatar_url")
                    .HasMaxLength(255);
                entity.Property(e => e.LastLogin)
                    .HasColumnName("last_login");
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                // Unique constraints
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.UserName).IsUnique();
                // Indexes
                entity.HasIndex(e => e.Role);
                entity.HasIndex(e => e.CampusId);
                entity.HasIndex(e => e.Status);

                // Relationship: User -> Campus
                entity.HasOne(e => e.Campus)
                    .WithMany(c => c.Users)
                    .HasForeignKey(e => e.CampusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Facility configuration
            modelBuilder.Entity<Facility>(entity =>
            {
                entity.ToTable("facility");
                entity.HasKey(e => e.FacilityId);
                entity.Property(e => e.FacilityId)
                    .HasColumnName("facility_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");
                entity.Property(e => e.Capacity)
                    .HasColumnName("capacity")
                    .IsRequired();
                entity.Property(e => e.RoomNumber)
                    .HasColumnName("room_number")
                    .HasMaxLength(50);
                entity.Property(e => e.FloorNumber)
                    .HasColumnName("floor_number")
                    .HasMaxLength(50);
                entity.Property(e => e.CampusId)
                    .HasColumnName("campus_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.TypeId)
                    .HasColumnName("type_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(e => e.Amenities)
                    .HasColumnName("amenities")
                    .HasColumnType("nvarchar(max)"); // JSON
                entity.Property(e => e.FacilityManagerId)
                    .HasColumnName("facility_manager_id")
                    .HasMaxLength(6);
                entity.Property(e => e.MaxConcurrentBookings)
                    .HasColumnName("max_concurrent_bookings")
                    .HasDefaultValue(1);
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                // Unique constraint: campus_id + name
                entity.HasIndex(e => new { e.CampusId, e.Name }).IsUnique();
                // Indexes
                entity.HasIndex(e => e.CampusId);
                entity.HasIndex(e => e.TypeId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Name);

                // Relationships
                entity.HasOne(e => e.Campus)
                    .WithMany(c => c.Facilities)
                    .HasForeignKey(e => e.CampusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.FacilityType)
                    .WithMany(ft => ft.Facilities)
                    .HasForeignKey(e => e.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.FacilityManager)
                    .WithMany(u => u.ManagedFacilities)
                    .HasForeignKey(e => e.FacilityManagerId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // FacilityImage configuration
            modelBuilder.Entity<FacilityImage>(entity =>
            {
                entity.ToTable("facility_images");
                entity.HasKey(e => e.ImageId);
                entity.Property(e => e.ImageId)
                    .HasColumnName("image_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.FacilityId)
                    .HasColumnName("facility_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.ImageUrl)
                    .HasColumnName("image_url")
                    .HasMaxLength(255)
                    .IsRequired();
                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(255);
                entity.Property(e => e.UploadDate)
                    .HasColumnName("upload_date")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ImageOrder)
                    .HasColumnName("image_order");

                // Relationship
                entity.HasOne(e => e.Facility)
                    .WithMany(f => f.FacilityImages)
                    .HasForeignKey(e => e.FacilityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Booking configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("booking");
                entity.HasKey(e => e.BookingId);
                entity.Property(e => e.BookingId)
                    .HasColumnName("booking_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.FacilityId)
                    .HasColumnName("facility_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time")
                    .IsRequired();
                entity.Property(e => e.EndTime)
                    .HasColumnName("end_time")
                    .IsRequired();
                entity.Property(e => e.Purpose)
                    .HasColumnName("purpose")
                    .HasMaxLength(255);
                entity.Property(e => e.Category)
                    .HasColumnName("category")
                    .HasMaxLength(100);
                entity.Property(e => e.EstimatedAttendees)
                    .HasColumnName("estimated_attendees");
                entity.Property(e => e.SpecialRequirements)
                    .HasColumnName("special_requirements")
                    .HasColumnType("nvarchar(max)"); // JSON
                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion<string>()
                    .HasDefaultValue(BookingStatus.Draft);
                entity.Property(e => e.ApprovedBy)
                    .HasColumnName("approved_by")
                    .HasMaxLength(6);
                entity.Property(e => e.ApprovedAt)
                    .HasColumnName("approved_at");
                entity.Property(e => e.RejectionReason)
                    .HasColumnName("rejection_reason")
                    .HasMaxLength(255);
                entity.Property(e => e.CheckInTime)
                    .HasColumnName("check_in_time");
                entity.Property(e => e.CheckOutTime)
                    .HasColumnName("check_out_time");
                entity.Property(e => e.IsUsed)
                    .HasColumnName("is_used")
                    .HasDefaultValue(false);
                entity.Property(e => e.CancelledAt)
                    .HasColumnName("cancelled_at");
                entity.Property(e => e.CancellationReason)
                    .HasColumnName("cancellation_reason")
                    .HasMaxLength(255);
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                // Critical index for conflict detection
                entity.HasIndex(e => new { e.FacilityId, e.StartTime, e.EndTime, e.Status });

                // Relationships
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Facility)
                    .WithMany(f => f.Bookings)
                    .HasForeignKey(e => e.FacilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Approver)
                    .WithMany(u => u.ApprovedBookings)
                    .HasForeignKey(e => e.ApprovedBy)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // BookingHistory configuration
            modelBuilder.Entity<BookingHistory>(entity =>
            {
                entity.ToTable("booking_history");
                entity.HasKey(e => e.HistoryId);
                entity.Property(e => e.HistoryId)
                    .HasColumnName("history_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.BookingId)
                    .HasColumnName("booking_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.FieldChanged)
                    .HasColumnName("field_changed")
                    .HasMaxLength(100);
                entity.Property(e => e.OldValue)
                    .HasColumnName("old_value")
                    .HasColumnType("text");
                entity.Property(e => e.NewValue)
                    .HasColumnName("new_value")
                    .HasColumnType("text");
                entity.Property(e => e.Reason)
                    .HasColumnName("reason")
                    .HasMaxLength(255);
                entity.Property(e => e.ChangedAt)
                    .HasColumnName("changed_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(e => e.Booking)
                    .WithMany(b => b.BookingHistories)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ModifiedByUser)
                    .WithMany(u => u.BookingHistories)
                    .HasForeignKey(e => e.ModifiedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // FacilityMaintenance configuration
            modelBuilder.Entity<FacilityMaintenance>(entity =>
            {
                entity.ToTable("facility_maintenance");
                entity.HasKey(e => e.MaintenanceId);
                entity.Property(e => e.MaintenanceId)
                    .HasColumnName("maintenance_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.FacilityId)
                    .HasColumnName("facility_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.IssueType)
                    .HasColumnName("issue_type")
                    .HasMaxLength(100);
                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");
                entity.Property(e => e.Priority)
                    .HasColumnName("priority")
                    .HasConversion<string>()
                    .HasDefaultValue(MaintenancePriority.Medium);
                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(e => e.AssignedTo)
                    .HasColumnName("assigned_to")
                    .HasMaxLength(6);
                entity.Property(e => e.ScheduledStart)
                    .HasColumnName("scheduled_start");
                entity.Property(e => e.ScheduledEnd)
                    .HasColumnName("scheduled_end");
                entity.Property(e => e.ActualStart)
                    .HasColumnName("actual_start");
                entity.Property(e => e.ActualEnd)
                    .HasColumnName("actual_end");
                entity.Property(e => e.CompletionNotes)
                    .HasColumnName("completion_notes")
                    .HasColumnType("text");
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(e => e.Facility)
                    .WithMany(f => f.Maintenances)
                    .HasForeignKey(e => e.FacilityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.AssignedToUser)
                    .WithMany(u => u.AssignedMaintenances)
                    .HasForeignKey(e => e.AssignedTo)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // BookingFeedback configuration
            modelBuilder.Entity<BookingFeedback>(entity =>
            {
                entity.ToTable("booking_feedback");
                entity.HasKey(e => e.FeedbackId);
                entity.Property(e => e.FeedbackId)
                    .HasColumnName("feedback_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.BookingId)
                    .HasColumnName("booking_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.Rating)
                    .HasColumnName("rating")
                    .IsRequired();
                entity.Property(e => e.Comments)
                    .HasColumnName("comments")
                    .HasColumnType("text");
                entity.Property(e => e.ReportIssue)
                    .HasColumnName("report_issue")
                    .HasDefaultValue(false);
                entity.Property(e => e.IssueDescription)
                    .HasColumnName("issue_description")
                    .HasColumnType("text");
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ResolvedAt)
                    .HasColumnName("resolved_at");

                // Relationships
                entity.HasOne(e => e.Booking)
                    .WithMany(b => b.BookingFeedbacks)
                    .HasForeignKey(e => e.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.BookingFeedbacks)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notification");
                entity.HasKey(e => e.NotificationId);
                entity.Property(e => e.NotificationId)
                    .HasColumnName("notification_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(255);
                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasColumnType("text");
                entity.Property(e => e.RelatedEntityType)
                    .HasColumnName("related_entity_type")
                    .HasMaxLength(100);
                entity.Property(e => e.RelatedEntityId)
                    .HasColumnName("related_entity_id");
                entity.Property(e => e.IsRead)
                    .HasColumnName("is_read")
                    .HasDefaultValue(false);
                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ReadAt)
                    .HasColumnName("read_at");

                // Indexes
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.IsRead);
                entity.HasIndex(e => e.CreatedAt).IsDescending();

                // Relationship
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Report configuration
            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("report");
                entity.HasKey(e => e.ReportId);
                entity.Property(e => e.ReportId)
                    .HasColumnName("report_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.ReportName)
                    .HasColumnName("report_name")
                    .HasMaxLength(255)
                    .IsRequired();
                entity.Property(e => e.ReportType)
                    .HasColumnName("report_type")
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(e => e.CampusId)
                    .HasColumnName("campus_id")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.FacilityId)
                    .HasColumnName("facility_id")
                    .HasMaxLength(6);
                entity.Property(e => e.ReportPeriodStart)
                    .HasColumnName("report_period_start")
                    .IsRequired();
                entity.Property(e => e.ReportPeriodEnd)
                    .HasColumnName("report_period_end")
                    .IsRequired();
                entity.Property(e => e.PeriodType)
                    .HasColumnName("period_type")
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(e => e.TotalBookings)
                    .HasColumnName("total_bookings")
                    .HasDefaultValue(0);
                entity.Property(e => e.ApprovedBookings)
                    .HasColumnName("approved_bookings")
                    .HasDefaultValue(0);
                entity.Property(e => e.RejectedBookings)
                    .HasColumnName("rejected_bookings")
                    .HasDefaultValue(0);
                entity.Property(e => e.CompletedBookings)
                    .HasColumnName("completed_bookings")
                    .HasDefaultValue(0);
                entity.Property(e => e.UtilizationRate)
                    .HasColumnName("utilization_rate")
                    .HasColumnType("decimal(5,2)")
                    .HasDefaultValue(0);
                entity.Property(e => e.AvgRating)
                    .HasColumnName("avg_rating")
                    .HasColumnType("decimal(3,2)")
                    .HasDefaultValue(0);
                entity.Property(e => e.UniqueUsers)
                    .HasColumnName("unique_users")
                    .HasDefaultValue(0);
                entity.Property(e => e.PeakHours)
                    .HasColumnName("peak_hours")
                    .HasMaxLength(255);
                entity.Property(e => e.BusiestDay)
                    .HasColumnName("busiest_day")
                    .HasMaxLength(50);
                entity.Property(e => e.GeneratedBy)
                    .HasColumnName("generated_by")
                    .HasMaxLength(6)
                    .IsRequired();
                entity.Property(e => e.GeneratedAt)
                    .HasColumnName("generated_at")
                    .HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsPublished)
                    .HasColumnName("is_published")
                    .HasDefaultValue(false);
                entity.Property(e => e.Notes)
                    .HasColumnName("notes")
                    .HasColumnType("text");

                // Relationships
                entity.HasOne(e => e.Campus)
                    .WithMany(c => c.Reports)
                    .HasForeignKey(e => e.CampusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Facility)
                    .WithMany(f => f.Reports)
                    .HasForeignKey(e => e.FacilityId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.GeneratedByUser)
                    .WithMany(u => u.GeneratedReports)
                    .HasForeignKey(e => e.GeneratedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

