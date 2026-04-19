using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Features.Data.Models;

namespace StudentManagementSystem.Features.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student → Course (major/program)
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Course)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Enrollment → Student
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Enrollment → Course
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Grade → Enrollment (one-to-one)
        modelBuilder.Entity<Grade>()
            .HasOne(g => g.Enrollment)
            .WithOne(e => e.Grade)
            .HasForeignKey<Grade>(g => g.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // User → Student (optional)
        modelBuilder.Entity<Student>()
            .HasOne(s => s.User)
            .WithOne(u => u.Student)
            .HasForeignKey<User>(u => u.StudentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Unique constraints
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentNumber)
            .IsUnique();

        modelBuilder.Entity<Course>()
            .HasIndex(c => c.CourseCode)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}
