using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRecruitment_Project.Models;

namespace SmartRecruitment_Project.Data.Configurations
{
    public class ApplicationConfiguration
        : IEntityTypeConfiguration<Application>
    {
        public void Configure(
            EntityTypeBuilder<Application> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MatchScore)
                .HasPrecision(5, 2);

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.AppliedAt)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.JobVacancyId,
                x.JobSeekerProfileId
            })
            .IsUnique();

            builder.HasOne(x => x.JobVacancy)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.JobVacancyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.JobSeekerProfile)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.JobSeekerProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}