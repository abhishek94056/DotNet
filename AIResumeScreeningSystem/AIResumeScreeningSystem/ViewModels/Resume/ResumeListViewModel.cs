namespace AIResumeScreeningSystem.ViewModels.Resume
{
    public class ResumeListViewModel
    {
        public List<ResumeViewModel> Resumes { get; set; } = new();
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public int TotalCount => Resumes.Count;
        public int ParsedCount => Resumes.Count(r => r.Status == Models.ResumeStatus.Parsed);
        public int PendingCount => Resumes.Count(r =>
            r.Status == Models.ResumeStatus.Uploaded ||
            r.Status == Models.ResumeStatus.Parsing);
    }
}