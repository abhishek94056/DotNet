using AIResumeScreeningSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeScreeningSystem.Configurations.EntityConfigurations
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasKey(j => j.Id);

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Company)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Description)
                .IsRequired();

            builder.Property(j => j.SalaryMin)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.SalaryMax)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(j => j.PostedBy)
                .WithMany()
                .HasForeignKey(j => j.PostedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(j => j.JobSkills)
                .WithOne(js => js.Job)
                .HasForeignKey(js => js.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(j => j.Applications)
                .WithOne(a => a.Job)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(j => j.InterviewQuestions)
                .WithOne(iq => iq.Job)
                .HasForeignKey(iq => iq.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(j => j.Status);
            builder.HasIndex(j => j.PostedDate);
        }
    }
}