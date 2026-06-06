using AIResumeScreeningSystem.DTOs;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface IResumeParserService
    {
        Task<ResumeParseResultDto> ParseAsync(string filePath, string fileExtension);
        ResumeParseResultDto ParsePdf(string filePath);
        ResumeParseResultDto ParseDocx(string filePath);
        List<string> ExtractSkills(string rawText);
        string? ExtractEmail(string text);
        string? ExtractPhone(string text);
        string? ExtractName(string text);
        string ExtractSection(string text, string[] sectionHeaders, string[] nextSectionHeaders);
        int EstimateExperienceYears(string text);
        string? InferHighestEducation(string text);
    }
}