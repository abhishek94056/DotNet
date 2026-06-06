using AIResumeScreeningSystem.Configurations.EntityConfigurations;
using AIResumeScreeningSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AIResumeScreeningSystem.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<InterviewQuestion> InterviewQuestions { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all entity configurations
            builder.ApplyConfiguration(new ApplicationUserConfiguration());
            builder.ApplyConfiguration(new JobConfiguration());
            builder.ApplyConfiguration(new CandidateConfiguration());
            builder.ApplyConfiguration(new ResumeConfiguration());
            builder.ApplyConfiguration(new ApplicationConfiguration());
            builder.ApplyConfiguration(new SkillConfiguration());
            builder.ApplyConfiguration(new CandidateSkillConfiguration());
            builder.ApplyConfiguration(new JobSkillConfiguration());
            builder.ApplyConfiguration(new NotificationConfiguration());
            builder.ApplyConfiguration(new InterviewQuestionConfiguration());
            builder.ApplyConfiguration(new ReportConfiguration());

            // Rename Identity tables for clarity
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().ToTable("Roles");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("UserTokens");

            // Seed default skills
            builder.Entity<Skill>().HasData(
                new Skill { Id = 1, Name = "C#", Category = SkillCategory.Technical, CreatedAt = DateTime.UtcNow },
                new Skill { Id = 2, Name = "ASP.NET Core", Category = SkillCategory.Framework, CreatedAt = DateTime.UtcNow },
                new Skill { Id = 3, Name = "SQL Server", Category = SkillCategory.Technical, CreatedAt = DateTime.UtcNow },
                new Skill { Id = 4, Name = "JavaScript", Category = SkillCategory.Technical, CreatedAt = DateTime.UtcNow },
                new Skill { Id = 5, Name = "React", Category = SkillCategory.Framework, CreatedAt = DateTime.UtcNow },
                new Skill { Id = 6, Name = "Python", Category = SkillCategory.Technical, CreatedAt = DateTime.UtcNow },
                new Skill { Id = 7, Name = "Azure", Category = SkillCategory.Tool, CreatedAt = DateTime.UtcNow },
                new Skill { Id = 8, Name = "Docker", Category = SkillCategory.Tool, CreatedAt = DateTime.UtcNow },
                new Skill { Id = 9, Name = "Git", Category = SkillCategory.Tool, CreatedAt = DateTime.UtcNow },
                new Skill { Id = 10, Name = "Communication", Category = SkillCategory.Soft, CreatedAt = DateTime.UtcNow }
            );
        }
    }
}