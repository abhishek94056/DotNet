using AIResumeScreeningSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIResumeScreeningSystem.Configurations.EntityConfigurations
{
    public class InterviewQuestionConfiguration : IEntityTypeConfiguration<InterviewQuestion>
    {
        public void Configure(EntityTypeBuilder<InterviewQuestion> builder)
        {
            builder.HasKey(iq => iq.Id);

            //builder.HasOne(iq => iq.Job)
            //    .WithMany(j => j.InterviewQuestions)
            //    .HasForeignKey(iq => iq.JobId)
            //    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(iq => iq.Job)
               .WithMany(j => j.InterviewQuestions)
               .HasForeignKey(iq => iq.JobId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(iq => iq.Application)
                .WithMany()
                .HasForeignKey(iq => iq.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}