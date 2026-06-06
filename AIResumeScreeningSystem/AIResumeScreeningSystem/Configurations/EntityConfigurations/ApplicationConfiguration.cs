using AIResumeScreeningSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeScreeningSystem.Configurations.EntityConfigurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.AIMatchScore)
                .HasColumnType("decimal(5,2)");

            builder.Property(a => a.SkillMatchPercentage)
                .HasColumnType("decimal(5,2)");

            builder.HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Candidate)
                .WithMany(c => c.Applications)
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Resume)
                .WithMany(r => r.Applications)
                .HasForeignKey(a => a.ResumeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ReviewedBy)
                .WithMany()
                .HasForeignKey(a => a.ReviewedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Prevent duplicate applications
            builder.HasIndex(a => new { a.JobId, a.CandidateId }).IsUnique();

            builder.HasIndex(a => a.Status);
            builder.HasIndex(a => a.AIMatchScore);
        }
    }
}