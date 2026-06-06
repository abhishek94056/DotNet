namespace AIResumeScreeningSystem.ViewModels.Candidate
{
    public class CandidateListViewModel
    {
        public List<CandidateViewModel> Candidates { get; set; } = new();
        public CandidateSearchViewModel Search { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 12;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // Summary stats
        public int TotalCandidates { get; set; }
        public int AvailableCandidates { get; set; }
        public int NewThisMonthCount { get; set; }
    }
}