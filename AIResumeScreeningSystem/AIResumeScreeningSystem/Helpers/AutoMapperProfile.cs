using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.ViewModels.Account;
using AIResumeScreeningSystem.ViewModels.Candidate;
using AIResumeScreeningSystem.ViewModels.Job;
using AIResumeScreeningSystem.ViewModels.Resume;
using AutoMapper;

namespace AIResumeScreeningSystem.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ── User / Account ─────────────────────────────────────────────
            CreateMap<RegisterViewModel, ApplicationUser>()
                .ForMember(d => d.UserName,
                    o => o.MapFrom(s => s.Email))
                .ForMember(d => d.CreatedAt,
                    o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.IsActive,
                    o => o.MapFrom(_ => true));

            CreateMap<ApplicationUser, RegisterViewModel>()
                .ForMember(d => d.Password, o => o.Ignore())
                .ForMember(d => d.ConfirmPassword, o => o.Ignore())
                .ForMember(d => d.AgreeToTerms, o => o.Ignore())
                .ForMember(d => d.Role, o => o.Ignore());

            // ── Candidate ──────────────────────────────────────────────────
            CreateMap<Candidate, CandidateViewModel>()
                .ForMember(d => d.FullName,
                    o => o.MapFrom(s =>
                        $"{s.User.FirstName} {s.User.LastName}"))
                .ForMember(d => d.Email,
                    o => o.MapFrom(s => s.User.Email ?? string.Empty))
                .ForMember(d => d.PhoneNumber,
                    o => o.MapFrom(s => s.User.PhoneNumber))
                .ForMember(d => d.IsActive,
                    o => o.MapFrom(s => s.User.IsActive))
                .ForMember(d => d.ProfileImagePath,
                    o => o.MapFrom(s => s.User.ProfileImagePath))
                .ForMember(d => d.Skills,
                    o => o.MapFrom(s => s.CandidateSkills))
                .ForMember(d => d.TotalApplications,
                    o => o.MapFrom(s => s.Applications.Count))
                .ForMember(d => d.TotalResumes,
                    o => o.MapFrom(s => s.Resumes.Count))
                .ForMember(d => d.ShortlistedCount,
                    o => o.MapFrom(s =>
                        s.Applications.Count(a =>
                            a.Status == ApplicationStatus.Shortlisted)));

            CreateMap<CandidateSkill, CandidateSkillViewModel>()
                .ForMember(d => d.SkillName,
                    o => o.MapFrom(s => s.Skill.Name))
                .ForMember(d => d.Category,
                    o => o.MapFrom(s => s.Skill.Category));

            CreateMap<CandidateProfileViewModel, Candidate>()
                .ForMember(d => d.UpdatedAt,
                    o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.User, o => o.Ignore())
                .ForMember(d => d.UserId, o => o.Ignore())
                .ForMember(d => d.Applications, o => o.Ignore())
                .ForMember(d => d.Resumes, o => o.Ignore())
                .ForMember(d => d.CandidateSkills, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore());

            // ── Job ────────────────────────────────────────────────────────
            CreateMap<Job, JobViewModel>()
                .ForMember(d => d.PostedByName,
                    o => o.MapFrom(s =>
                        s.PostedBy != null
                            ? $"{s.PostedBy.FirstName} {s.PostedBy.LastName}"
                            : "Unknown"))
                .ForMember(d => d.TotalApplications,
                    o => o.MapFrom(s => s.Applications.Count))
                .ForMember(d => d.RequiredSkills,
                    o => o.MapFrom(s =>
                        s.JobSkills.Where(js => js.IsRequired)
                            .Select(js => js.Skill.Name).ToList()))
                .ForMember(d => d.OptionalSkills,
                    o => o.MapFrom(s =>
                        s.JobSkills.Where(js => !js.IsRequired)
                            .Select(js => js.Skill.Name).ToList()));

            CreateMap<CreateJobViewModel, Job>()
                .ForMember(d => d.PostedDate,
                    o => o.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.JobSkills, o => o.Ignore())
                .ForMember(d => d.Applications, o => o.Ignore())
                .ForMember(d => d.InterviewQuestions, o => o.Ignore())
                .ForMember(d => d.PostedBy, o => o.Ignore())
                .ForMember(d => d.PostedByUserId, o => o.Ignore());

            CreateMap<Job, EditJobViewModel>()
                .ForMember(d => d.RequiredSkillIds,
                    o => o.MapFrom(s =>
                        s.JobSkills.Where(js => js.IsRequired)
                            .Select(js => js.SkillId).ToList()))
                .ForMember(d => d.OptionalSkillIds,
                    o => o.MapFrom(s =>
                        s.JobSkills.Where(js => !js.IsRequired)
                            .Select(js => js.SkillId).ToList()))
                .ForMember(d => d.AvailableSkills, o => o.Ignore())
                .ForMember(d => d.JobTypeOptions, o => o.Ignore())
                .ForMember(d => d.StatusOptions, o => o.Ignore());

            // ── Resume ─────────────────────────────────────────────────────
            CreateMap<Resume, ResumeViewModel>()
                .ForMember(d => d.CandidateName,
                    o => o.MapFrom(s =>
                        s.Candidate != null && s.Candidate.User != null
                            ? $"{s.Candidate.User.FirstName} {s.Candidate.User.LastName}"
                            : string.Empty));
        }
    }
}