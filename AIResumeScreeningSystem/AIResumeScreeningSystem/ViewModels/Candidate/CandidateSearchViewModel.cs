namespace AIResumeScreeningSystem.ViewModels.Candidate
{
    public class CandidateSearchViewModel
    {
        public string? Keyword { get; set; }
        public string? Location { get; set; }
        public string? Skill { get; set; }
        public int? MinExperience { get; set; }
        public int? MaxExperience { get; set; }
        public string? Education { get; set; }
        public bool? IsAvailable { get; set; }
        public string SortBy { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}