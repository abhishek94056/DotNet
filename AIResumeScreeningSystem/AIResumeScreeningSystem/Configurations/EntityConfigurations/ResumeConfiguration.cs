using AIResumeScreeningSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeScreeningSystem.Configurations.EntityConfigurations
{
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.FileName).IsRequired().HasMaxLength(500);
            builder.Property(r => r.FilePath).IsRequired().HasMaxLength(1000);

            builder.HasOne(r => r.Candidate)
                .WithMany(c => c.Resumes)
                .HasForeignKey(r => r.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}