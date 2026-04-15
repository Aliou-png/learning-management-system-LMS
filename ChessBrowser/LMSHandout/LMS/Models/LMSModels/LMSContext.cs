using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class LMSContext : DbContext
    {
        public LMSContext()
        {
        }

        public LMSContext(DbContextOptions<LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrator> Administrators { get; set; } = null!;
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; } = null!;
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; } = null!;
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; } = null!;
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<AssignmentCategory> AssignmentCategories { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<EfmigrationsHistory> EfmigrationsHistories { get; set; } = null!;
        public virtual DbSet<Enrolled> Enrolleds { get; set; } = null!;
        public virtual DbSet<Professor> Professors { get; set; } = null!;
        public virtual DbSet<Sshkey> Sshkeys { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Submission> Submissions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=LMS:LMSConnectionString", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.16-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Administrator>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("first_Name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("last_Name");
            });

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.AccessFailedCount).HasColumnType("int(11)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.LockoutEnd).HasMaxLength(6);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "AspNetUserRole",
                        l => l.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                        r => r.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("AspNetUserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => new { e.Name, e.CategoryId }, "Name")
                    .IsUnique();

                entity.HasIndex(e => e.CategoryId, "fk_assignment_category");

                entity.Property(e => e.AssignmentId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("assignmentID");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("categoryID");

                entity.Property(e => e.Content).HasMaxLength(8192);

                entity.Property(e => e.Due).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Points).HasColumnType("smallint(5) unsigned");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("fk_assignment_category");
            });

            modelBuilder.Entity<AssignmentCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PRIMARY");

                entity.ToTable("Assignment_Categories");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => new { e.ClassId, e.CategoryId }, "ClassID")
                    .IsUnique();

                entity.Property(e => e.CategoryId)
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever()
                    .HasColumnName("CategoryID");

                entity.Property(e => e.Category).HasMaxLength(100);

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("ClassID");

                entity.Property(e => e.Weight).HasColumnType("tinyint(3) unsigned");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_category_class");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => e.Professor, "Professor");

                entity.HasIndex(e => new { e.CourseId, e.SemesterYear, e.SemesterSeason }, "courseID")
                    .IsUnique();

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("classID");

                entity.Property(e => e.CourseId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("courseID");

                entity.Property(e => e.End).HasColumnType("time");

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.Professor)
                    .HasMaxLength(8)
                    .IsFixedLength();

                entity.Property(e => e.SemesterSeason)
                    .HasMaxLength(10)
                    .HasColumnName("semester_season");

                entity.Property(e => e.SemesterYear)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("semester_year");

                entity.Property(e => e.Start).HasColumnType("time");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CourseId)
                    .HasConstraintName("fk_classes_course");

                entity.HasOne(d => d.ProfessorNavigation)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.Professor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_1");
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => new { e.Num, e.Subject }, "Num")
                    .IsUnique();

                entity.HasIndex(e => e.Subject, "Subject");

                entity.Property(e => e.CourseId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("courseID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Num).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Subject).HasMaxLength(4);

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Subject)
                    .HasName("PRIMARY");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.Subject).HasMaxLength(4);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<EfmigrationsHistory>(entity =>
            {
                entity.HasKey(e => e.MigrationId)
                    .HasName("PRIMARY");

                entity.ToTable("__EFMigrationsHistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ProductVersion).HasMaxLength(32);
            });

            modelBuilder.Entity<Enrolled>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.ClassId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("Enrolled");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => e.ClassId, "fk_enrolled_class");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("ClassID");

                entity.Property(e => e.Grade).HasMaxLength(2);

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Enrolleds)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_enrolled_class");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.Enrolleds)
                    .HasForeignKey(d => d.UId)
                    .HasConstraintName("fk_enrolled_student");
            });

            modelBuilder.Entity<Professor>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => e.Subject, "Subject");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("first_Name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("last_Name");

                entity.Property(e => e.Subject).HasMaxLength(4);

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_2");
            });

            modelBuilder.Entity<Sshkey>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("sshkey");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.Property(e => e.Sshkey1)
                    .HasColumnType("text")
                    .HasColumnName("sshkey");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => e.Subject, "Subject");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .HasColumnName("first_Name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasColumnName("last_Name");

                entity.Property(e => e.Subject).HasMaxLength(4);

                entity.HasOne(d => d.SubjectNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Subject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_2");
            });

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(e => new { e.AssignmentId, e.UId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.HasCharSet("latin1")
                    .UseCollation("latin1_swedish_ci");

                entity.HasIndex(e => e.UId, "fk_submission_student");

                entity.Property(e => e.AssignmentId)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("assignmentID");

                entity.Property(e => e.UId)
                    .HasMaxLength(8)
                    .HasColumnName("uID")
                    .IsFixedLength();

                entity.Property(e => e.Content).HasMaxLength(8192);

                entity.Property(e => e.Score).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Submitted).HasColumnType("datetime");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.AssignmentId)
                    .HasConstraintName("fk_submission_assignment");

                entity.HasOne(d => d.UIdNavigation)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.UId)
                    .HasConstraintName("fk_submission_student");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
