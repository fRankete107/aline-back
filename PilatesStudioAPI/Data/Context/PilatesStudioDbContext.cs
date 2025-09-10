using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PilatesStudioAPI.Models.Entities;

namespace PilatesStudioAPI.Data.Context;

public class PilatesStudioDbContext : IdentityDbContext<User, IdentityRole<long>, long>
{
    public PilatesStudioDbContext(DbContextOptions<PilatesStudioDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Identity tables with custom names
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
        });

        modelBuilder.Entity<IdentityRole<long>>(entity =>
        {
            entity.ToTable("roles");
        });

        modelBuilder.Entity<IdentityUserRole<long>>(entity =>
        {
            entity.ToTable("user_roles");
        });

        modelBuilder.Entity<IdentityUserClaim<long>>(entity =>
        {
            entity.ToTable("user_claims");
        });

        modelBuilder.Entity<IdentityUserLogin<long>>(entity =>
        {
            entity.ToTable("user_logins");
        });

        modelBuilder.Entity<IdentityRoleClaim<long>>(entity =>
        {
            entity.ToTable("role_claims");
        });

        modelBuilder.Entity<IdentityUserToken<long>>(entity =>
        {
            entity.ToTable("user_tokens");
        });

        // Configure custom entities
        ConfigureInstructor(modelBuilder);
        ConfigureStudent(modelBuilder);
        ConfigurePlan(modelBuilder);
        ConfigureSubscription(modelBuilder);
        ConfigureZone(modelBuilder);
        ConfigureClass(modelBuilder);
        ConfigureReservation(modelBuilder);
        ConfigurePayment(modelBuilder);
        ConfigureContact(modelBuilder);
        ConfigureAuditLog(modelBuilder);

        // Configure unique constraints
        ConfigureUniqueConstraints(modelBuilder);

        // Configure indexes
        ConfigureIndexes(modelBuilder);
    }

    private void ConfigureInstructor(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.ToTable("instructors");
            
            entity.HasOne(e => e.User)
                  .WithOne(u => u.Instructor)
                  .HasForeignKey<Instructor>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureStudent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("students");
            
            entity.HasOne(e => e.User)
                  .WithOne(u => u.Student)
                  .HasForeignKey<Student>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigurePlan(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Plan>(entity =>
        {
            entity.ToTable("plans");
        });
    }

    private void ConfigureSubscription(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.ToTable("subscriptions");
            
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Subscriptions)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Plan)
                  .WithMany(p => p.Subscriptions)
                  .HasForeignKey(e => e.PlanId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureZone(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Zone>(entity =>
        {
            entity.ToTable("zones");
        });
    }

    private void ConfigureClass(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.ToTable("classes");
            
            entity.HasOne(e => e.Instructor)
                  .WithMany(i => i.Classes)
                  .HasForeignKey(e => e.InstructorId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Zone)
                  .WithMany(z => z.Classes)
                  .HasForeignKey(e => e.ZoneId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureReservation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("reservations");
            
            entity.HasOne(e => e.Class)
                  .WithMany(c => c.Reservations)
                  .HasForeignKey(e => e.ClassId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Reservations)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Subscription)
                  .WithMany(sub => sub.Reservations)
                  .HasForeignKey(e => e.SubscriptionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigurePayment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Payments)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasOne(e => e.Plan)
                  .WithMany(p => p.Payments)
                  .HasForeignKey(e => e.PlanId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureContact(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("contacts");
            
            entity.HasOne(e => e.AssignedUser)
                  .WithMany(u => u.AssignedContacts)
                  .HasForeignKey(e => e.AssignedTo)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureAuditLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_log");
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureUniqueConstraints(ModelBuilder modelBuilder)
    {
        // Unique constraint for class schedule (instructor, date, start_time)
        modelBuilder.Entity<Class>()
            .HasIndex(c => new { c.InstructorId, c.ClassDate, c.StartTime })
            .IsUnique()
            .HasDatabaseName("unique_class_schedule");

        // Unique constraint for student class reservation
        modelBuilder.Entity<Reservation>()
            .HasIndex(r => new { r.ClassId, r.StudentId })
            .IsUnique()
            .HasDatabaseName("unique_student_class");

        // Unique receipt number
        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.ReceiptNumber)
            .IsUnique()
            .HasDatabaseName("unique_receipt_number");
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Instructors indexes
        modelBuilder.Entity<Instructor>()
            .HasIndex(i => i.IsActive)
            .HasDatabaseName("idx_instructor_active");

        modelBuilder.Entity<Instructor>()
            .HasIndex(i => new { i.FirstName, i.LastName })
            .HasDatabaseName("idx_instructor_name");

        // Students indexes
        modelBuilder.Entity<Student>()
            .HasIndex(s => new { s.FirstName, s.LastName })
            .HasDatabaseName("idx_student_name");

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Phone)
            .HasDatabaseName("idx_student_phone");

        // Plans indexes
        modelBuilder.Entity<Plan>()
            .HasIndex(p => p.IsActive)
            .HasDatabaseName("idx_plan_active");

        modelBuilder.Entity<Plan>()
            .HasIndex(p => p.Price)
            .HasDatabaseName("idx_plan_price");

        // Subscriptions indexes
        modelBuilder.Entity<Subscription>()
            .HasIndex(s => s.StudentId)
            .HasDatabaseName("idx_subscription_student");

        modelBuilder.Entity<Subscription>()
            .HasIndex(s => s.Status)
            .HasDatabaseName("idx_subscription_status");

        modelBuilder.Entity<Subscription>()
            .HasIndex(s => s.ExpiryDate)
            .HasDatabaseName("idx_subscription_expiry");

        // Zones indexes
        modelBuilder.Entity<Zone>()
            .HasIndex(z => z.IsActive)
            .HasDatabaseName("idx_zone_active");

        // Classes indexes
        modelBuilder.Entity<Class>()
            .HasIndex(c => c.ClassDate)
            .HasDatabaseName("idx_class_date");

        modelBuilder.Entity<Class>()
            .HasIndex(c => c.InstructorId)
            .HasDatabaseName("idx_class_instructor");

        modelBuilder.Entity<Class>()
            .HasIndex(c => c.ZoneId)
            .HasDatabaseName("idx_class_zone");

        modelBuilder.Entity<Class>()
            .HasIndex(c => c.Status)
            .HasDatabaseName("idx_class_status");

        // Reservations indexes
        modelBuilder.Entity<Reservation>()
            .HasIndex(r => r.ClassId)
            .HasDatabaseName("idx_reservation_class");

        modelBuilder.Entity<Reservation>()
            .HasIndex(r => r.StudentId)
            .HasDatabaseName("idx_reservation_student");

        modelBuilder.Entity<Reservation>()
            .HasIndex(r => r.Status)
            .HasDatabaseName("idx_reservation_status");

        // Payments indexes
        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.StudentId)
            .HasDatabaseName("idx_payment_student");

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.PaymentDate)
            .HasDatabaseName("idx_payment_date");

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.Status)
            .HasDatabaseName("idx_payment_status");

        // Contacts indexes
        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Status)
            .HasDatabaseName("idx_contact_status");

        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.CreatedAt)
            .HasDatabaseName("idx_contact_date");

        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Email)
            .HasDatabaseName("idx_contact_email");

        // Audit log indexes
        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.TableName)
            .HasDatabaseName("idx_audit_table");

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.CreatedAt)
            .HasDatabaseName("idx_audit_date");

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.UserId)
            .HasDatabaseName("idx_audit_user");
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            try
            {
                if (entry.Property("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Added && entry.Property("CreatedAt") != null)
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
            catch
            {
                // Skip entities that don't have timestamp properties (like IdentityRole)
                continue;
            }
        }
    }
}