using Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace DataAccess
{
  public class ApplicationDbContext : IdentityDbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<CalendarEvent> CalendarEvents { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<CalendarRole> CalendarRoles { get; set; }
    public DbSet<CalendarAccess> CalendarAccesses { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<RecurringRule> RecurringRules { get; set; }
    public DbSet<ToDo> ToDos { get; set; }
    public DbSet<StudentRegistration> StudentRegistrations { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<AssignmentAttachment> AssignmentAttachments { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
    public DbSet<UserSignIns> UserSignIns { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<CalendarAccess>()
          .HasKey(ca => new { ca.ApplicationUserId, ca.CalendarId, ca.CalendarRoleId }); // Define composite key

      modelBuilder.Entity<CalendarAccess>()
          .HasOne(ca => ca.ApplicationUser)
          .WithMany()
          .HasForeignKey(ca => ca.ApplicationUserId);

      modelBuilder.Entity<CalendarAccess>()
          .HasOne(ca => ca.Calendar)
          .WithMany()
          .HasForeignKey(ca => ca.CalendarId);

      modelBuilder.Entity<CalendarAccess>()
          .HasOne(ca => ca.CalendarRole)
          .WithMany()
          .HasForeignKey(ca => ca.CalendarRoleId);

      modelBuilder.Entity<Class>()
          .HasOne(c => c.Instructor)
          .WithMany()
          .HasForeignKey(c => c.InstructorId)
          .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<RecurringRule>(entity =>
      {
        entity.HasKey(e => e.RecurringRuleId);
        entity.Property(e => e.RecurringRuleId).ValueGeneratedOnAdd();
      });

      modelBuilder.Entity<Event>(entity =>
      {
        entity.HasKey(e => e.EventId);
        entity.Property(e => e.EventId).ValueGeneratedOnAdd();
      });

      // Configures one-to-many relationships for StudentRegistration
      modelBuilder.Entity<StudentRegistration>()
          .HasOne(sr => sr.Student)
          .WithMany(u => u.Registrations)
          .HasForeignKey(sr => sr.StudentId);

      modelBuilder.Entity<StudentRegistration>()
          .HasOne(sr => sr.Class)
          .WithMany(c => c.Registrations)
          .HasForeignKey(sr => sr.ClassId);

      modelBuilder.Entity<Assignment>()
          .HasOne(a => a.ToDo)
          .WithMany()
          .HasForeignKey(a => a.ToDoId)
          .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<PaymentTransaction>()
          .HasOne(sr => sr.Student)
          .WithMany()
          .HasForeignKey(sr => sr.StudentId)
          .OnDelete(DeleteBehavior.NoAction);

      modelBuilder.Entity<PaymentTransaction>()
          .Property(p => p.Amount)
          .HasPrecision(18, 2);

      modelBuilder.Entity<UserSignIns>()
          .HasOne(u => u.User)
          .WithMany()
          .HasForeignKey(u => u.UserId)
          .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
    }
  }
}
