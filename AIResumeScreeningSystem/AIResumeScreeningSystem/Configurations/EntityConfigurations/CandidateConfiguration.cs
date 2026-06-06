using AIResumeScreeningSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeScreeningSystem.Configurations.EntityConfigurations
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.User)
                .WithOne(u => u.Candidate)
                .HasForeignKey<Candidate>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Resumes)
                .WithOne(r => r.Candidate)
                .HasForeignKey(r => r.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.CandidateSkills)
                .WithOne(cs => cs.Candidate)
                .HasForeignKey(cs => cs.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Applications)
                .WithOne(a => a.Candidate)
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}