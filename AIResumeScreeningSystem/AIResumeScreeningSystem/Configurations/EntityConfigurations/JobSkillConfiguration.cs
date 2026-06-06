using AIResumeScreeningSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeScreeningSystem.Configurations.EntityConfigurations
{
    public class JobSkillConfiguration : IEntityTypeConfiguration<JobSkill>
    {
        public void Configure(EntityTypeBuilder<JobSkill> builder)
        {
            builder.HasKey(js => js.Id);

            builder.HasIndex(js => new { js.JobId, js.SkillId }).IsUnique();

            builder.HasOne(js => js.Job)
                .WithMany(j => j.JobSkills)
                .HasForeignKey(js => js.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(js => js.Skill)
                .WithMany(s => s.JobSkills)
                .HasForeignKey(js => js.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}