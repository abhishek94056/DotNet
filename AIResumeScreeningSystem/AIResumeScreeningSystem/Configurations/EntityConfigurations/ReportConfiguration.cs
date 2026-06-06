using AIResumeScreeningSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeScreeningSystem.Configurations.EntityConfigurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.GeneratedBy)
                .WithMany()
                .HasForeignKey(r => r.GeneratedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Job)
                .WithMany()
                .HasForeignKey(r => r.JobId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}