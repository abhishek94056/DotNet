using System.ComponentModel.DataAnnotations;

namespace AIResumeScreeningSystem.ViewModels.Resume
{
    public class ResumeUploadViewModel
    {
        [Required(ErrorMessage = "Please select a resume file.")]
        [Display(Name = "Resume File")]
        public IFormFile? ResumeFile { get; set; }

        [Display(Name = "Set as Active Resume")]
        public bool SetAsActive { get; set; } = true;

        // Display info
        public List<ResumeViewModel> ExistingResumes { get; set; } = new();
        public int MaxFileSizeMB { get; set; } = 5;
        public List<string> AllowedExtensions { get; set; } = new() { ".pdf", ".docx", ".doc" };
    }
}