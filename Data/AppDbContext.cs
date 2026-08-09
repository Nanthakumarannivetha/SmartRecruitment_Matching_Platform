using Microsoft.EntityFrameworkCore;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // =========================
        // Database Tables
        // =========================

        public DbSet<User> Users => Set<User>();

        public DbSet<JobSeekerProfile> JobSeekerProfiles
            => Set<JobSeekerProfile>();

        public DbSet<EmployerProfile> EmployerProfiles
            => Set<EmployerProfile>();

        public DbSet<Skill> Skills
            => Set<Skill>();

        public DbSet<JobSeekerSkill> JobSeekerSkills
            => Set<JobSeekerSkill>();

        public DbSet<JobVacancy> JobVacancies
            => Set<JobVacancy>();

        public DbSet<JobVacancySkill> JobVacancySkills
            => Set<JobVacancySkill>();

        public DbSet<Application> Applications
            => Set<Application>();

        public DbSet<ContactRequest> ContactRequests
            => Set<ContactRequest>();

        public DbSet<Notification> Notifications
            => Set<Notification>();

        public DbSet<CvDocument> CvDocuments
            => Set<CvDocument>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =========================
            // User
            // =========================

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.Property(x => x.PasswordHash)
                    .IsRequired();

                entity.Property(x => x.Role)
                    .IsRequired();

                entity.Property(x => x.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(x => x.CreatedAt)
                    .IsRequired();
            });


            // =========================
            // Job Seeker Profile
            // =========================

            modelBuilder.Entity<JobSeekerProfile>(entity =>
            {
                entity.ToTable("JobSeekerProfiles");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Location)
                    .HasMaxLength(150);

                entity.Property(x => x.Summary)
                    .HasMaxLength(1000);

                entity.Property(x => x.YearsOfExperience)
                    .IsRequired();

                entity.Property(x => x.EducationLevel)
                    .IsRequired();

                entity.HasIndex(x => x.UserId)
                    .IsUnique();

                entity.HasOne(x => x.User)
                    .WithOne(x => x.JobSeekerProfile)
                    .HasForeignKey<JobSeekerProfile>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // Employer Profile
            // =========================

            modelBuilder.Entity<EmployerProfile>(entity =>
            {
                entity.ToTable("EmployerProfiles");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.CompanyName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Location)
                    .HasMaxLength(150);

                entity.Property(x => x.Description)
                    .HasMaxLength(1500);

                entity.Property(x => x.Website)
                    .HasMaxLength(300);

                entity.HasIndex(x => x.UserId)
                    .IsUnique();

                entity.HasOne(x => x.User)
                    .WithOne(x => x.EmployerProfile)
                    .HasForeignKey<EmployerProfile>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // Skill
            // =========================

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skills");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.NormalizedName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(x => x.NormalizedName)
                    .IsUnique();
            });


            // =========================
            // Job Seeker Skill
            // =========================

            modelBuilder.Entity<JobSeekerSkill>(entity =>
            {
                entity.ToTable("JobSeekerSkills");

                entity.HasKey(x => new
                {
                    x.JobSeekerProfileId,
                    x.SkillId
                });

                entity.HasOne(x => x.JobSeekerProfile)
                    .WithMany(x => x.JobSeekerSkills)
                    .HasForeignKey(x => x.JobSeekerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Skill)
                    .WithMany(x => x.JobSeekerSkills)
                    .HasForeignKey(x => x.SkillId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // Job Vacancy
            // =========================

            modelBuilder.Entity<JobVacancy>(entity =>
            {
                entity.ToTable("JobVacancies");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .IsRequired()
                    .HasMaxLength(3000);

                entity.Property(x => x.Location)
                    .HasMaxLength(150);

                entity.Property(x => x.MinimumExperienceYears)
                    .IsRequired();

                entity.Property(x => x.RequiredEducationLevel)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .IsRequired();

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.HasOne(x => x.EmployerProfile)
                    .WithMany(x => x.JobVacancies)
                    .HasForeignKey(x => x.EmployerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // Job Vacancy Skill
            // =========================

            modelBuilder.Entity<JobVacancySkill>(entity =>
            {
                entity.ToTable("JobVacancySkills");

                entity.HasKey(x => new
                {
                    x.JobVacancyId,
                    x.SkillId
                });

                entity.HasOne(x => x.JobVacancy)
                    .WithMany(x => x.JobVacancySkills)
                    .HasForeignKey(x => x.JobVacancyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Skill)
                    .WithMany(x => x.JobVacancySkills)
                    .HasForeignKey(x => x.SkillId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // Application
            // =========================

            modelBuilder.Entity<Application>(entity =>
            {
                entity.ToTable("Applications");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Status)
                    .IsRequired();

                entity.Property(x => x.MatchScore)
                    .HasPrecision(5, 2);

                entity.Property(x => x.AppliedAt)
                    .IsRequired();

                // Same candidate cannot apply twice
                // to the same vacancy.
                entity.HasIndex(x => new
                {
                    x.JobVacancyId,
                    x.JobSeekerProfileId
                })
                .IsUnique();

                entity.HasOne(x => x.JobVacancy)
                    .WithMany(x => x.Applications)
                    .HasForeignKey(x => x.JobVacancyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.JobSeekerProfile)
                    .WithMany(x => x.Applications)
                    .HasForeignKey(x => x.JobSeekerProfileId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =========================
            // Contact Request
            // =========================

            modelBuilder.Entity<ContactRequest>(entity =>
            {
                entity.ToTable("ContactRequests");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Message)
                    .HasMaxLength(1000);

                entity.Property(x => x.Status)
                    .IsRequired();

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.HasOne(x => x.Application)
                    .WithMany(x => x.ContactRequests)
                    .HasForeignKey(x => x.ApplicationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // Notification
            // =========================

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Message)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(x => x.Type)
                    .IsRequired();

                entity.Property(x => x.IsRead)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.Notifications)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // =========================
            // CV Document
            // =========================

            modelBuilder.Entity<CvDocument>(entity =>
            {
                entity.ToTable("CvDocuments");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.OriginalFileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(x => x.StoredFileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(x => x.ContentType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.FileSize)
                    .IsRequired();

                entity.Property(x => x.UploadedAt)
                    .IsRequired();

                // One Job Seeker = One current CV
                entity.HasIndex(x => x.JobSeekerProfileId)
                    .IsUnique();

                entity.HasOne(x => x.JobSeekerProfile)
                    .WithOne(x => x.CvDocument)
                    .HasForeignKey<CvDocument>(
                        x => x.JobSeekerProfileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}