using AIResumeScreeningSystem.DTOs;
using AIResumeScreeningSystem.Services.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class ResumeParserService : IResumeParserService
    {
        private readonly ILogger<ResumeParserService> _logger;

        // Common tech skills dictionary for matching
        private static readonly HashSet<string> KnownSkills = new(StringComparer.OrdinalIgnoreCase)
        {
            // Languages
            "C#","Java","Python","JavaScript","TypeScript","Go","Rust","Swift","Kotlin","PHP",
            "Ruby","Scala","R","MATLAB","Perl","Bash","PowerShell","SQL","NoSQL",
            // Frameworks & Libraries
            "ASP.NET","ASP.NET Core",".NET","React","Angular","Vue","Vue.js","Node.js","Express",
            "Django","Flask","Spring","Spring Boot","Laravel","Rails","FastAPI","Blazor",
            "Next.js","Nuxt.js","Bootstrap","Tailwind","jQuery","Redux",
            // Databases
            "SQL Server","MySQL","PostgreSQL","MongoDB","Redis","Cassandra","SQLite",
            "DynamoDB","Cosmos DB","Oracle","MariaDB","Elasticsearch","Firebase",
            // Cloud & DevOps
            "Azure","AWS","GCP","Google Cloud","Docker","Kubernetes","Terraform","Ansible",
            "Jenkins","GitHub Actions","CI/CD","ArgoCD","Helm","Prometheus","Grafana",
            // Tools & Platforms
            "Git","GitHub","GitLab","Bitbucket","JIRA","Confluence","Postman","Swagger",
            "Visual Studio","VS Code","IntelliJ","Eclipse","Xcode","Android Studio",
            // Architecture & Concepts
            "REST","GraphQL","gRPC","Microservices","SOLID","DDD","TDD","BDD","Agile","Scrum",
            "CQRS","Event Sourcing","OAuth","JWT","SAML","OpenID",
            // Testing
            "xUnit","NUnit","Jest","Mocha","Selenium","Cypress","Playwright","Postman",
            // Data & AI
            "Machine Learning","Deep Learning","TensorFlow","PyTorch","scikit-learn",
            "Pandas","NumPy","Power BI","Tableau","Spark","Hadoop","Kafka","Databricks",
            // Soft Skills
            "Leadership","Communication","Problem Solving","Team Player","Project Management",
            "Agile","Scrum","Kanban"
        };

        private static readonly string[] SkillSectionHeaders =
            { "skills", "technical skills", "core competencies", "technologies", "tech stack", "expertise" };

        private static readonly string[] EducationSectionHeaders =
            { "education", "academic background", "qualifications", "degree" };

        private static readonly string[] ExperienceSectionHeaders =
            { "experience", "work experience", "professional experience", "employment", "career history" };

        private static readonly string[] SummarySectionHeaders =
            { "summary", "profile", "objective", "about me", "professional summary", "overview" };

        private static readonly string[] AllSectionHeaders =
            { "skills", "technical skills", "core competencies", "education", "academic",
              "experience", "work experience", "professional experience", "employment",
              "projects", "certifications", "awards", "languages", "interests",
              "summary", "profile", "objective", "contact", "references" };

        public ResumeParserService(ILogger<ResumeParserService> logger)
        {
            _logger = logger;
        }

        // ─── Main Parse Entry ──────────────────────────────────────────────

        public async Task<ResumeParseResultDto> ParseAsync(string filePath, string fileExtension)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return fileExtension.ToLower() switch
                    {
                        ".pdf" => ParsePdf(filePath),
                        ".docx" => ParseDocx(filePath),
                        ".doc" => ParseDocxFallback(filePath),
                        _ => new ResumeParseResultDto
                        {
                            Success = false,
                            Error = $"Unsupported file type: {fileExtension}"
                        }
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing resume at {FilePath}", filePath);
                    return new ResumeParseResultDto
                    {
                        Success = false,
                        Error = $"Parsing failed: {ex.Message}"
                    };
                }
            });
        }

        // ─── PDF Parser (PdfPig) ───────────────────────────────────────────

        public ResumeParseResultDto ParsePdf(string filePath)
        {
            var result = new ResumeParseResultDto();
            var textBuilder = new StringBuilder();

            try
            {
                using var document = PdfDocument.Open(filePath);
                foreach (Page page in document.GetPages())
                {
                    var pageText = string.Join(" ",
                        page.GetWords().Select(w => w.Text));
                    textBuilder.AppendLine(pageText);
                }

                result.RawText = textBuilder.ToString();
                PopulateResult(result);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF parse error: {FilePath}", filePath);
                result.Success = false;
                result.Error = $"PDF parsing failed: {ex.Message}";
            }

            return result;
        }

        // ─── DOCX Parser (OpenXML) ─────────────────────────────────────────

        public ResumeParseResultDto ParseDocx(string filePath)
        {
            var result = new ResumeParseResultDto();

            try
            {
                using var wordDoc = WordprocessingDocument.Open(filePath, false);
                var body = wordDoc.MainDocumentPart?.Document?.Body;
                if (body == null)
                {
                    result.Success = false;
                    result.Error = "Could not read DOCX document body.";
                    return result;
                }

                var sb = new StringBuilder();
                foreach (var element in body.ChildElements)
                {
                    var text = element.InnerText?.Trim();
                    if (!string.IsNullOrWhiteSpace(text))
                        sb.AppendLine(text);
                }

                result.RawText = sb.ToString();
                PopulateResult(result);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DOCX parse error: {FilePath}", filePath);
                result.Success = false;
                result.Error = $"DOCX parsing failed: {ex.Message}";
            }

            return result;
        }

        // ─── DOC Fallback ─────────────────────────────────────────────────

        private ResumeParseResultDto ParseDocxFallback(string filePath)
        {
            // Attempt to read .doc as plain text (limited support)
            try
            {
                var text = File.ReadAllText(filePath, Encoding.UTF8);
                // Filter printable ASCII
                var cleaned = new string(text.Where(c => c >= 32 || c == '\n' || c == '\r').ToArray());
                var result = new ResumeParseResultDto { RawText = cleaned };
                PopulateResult(result);
                result.Success = true;
                return result;
            }
            catch
            {
                return new ResumeParseResultDto
                {
                    Success = false,
                    Error = ".doc format has limited support. Please convert to PDF or DOCX."
                };
            }
        }

        // ─── Core Population Logic ─────────────────────────────────────────

        private void PopulateResult(ResumeParseResultDto result)
        {
            if (string.IsNullOrWhiteSpace(result.RawText)) return;

            var text = result.RawText;

            result.Email = ExtractEmail(text);
            result.Phone = ExtractPhone(text);
            result.Name = ExtractName(text);

            result.SkillsSection = ExtractSection(text,
                SkillSectionHeaders, AllSectionHeaders.Except(SkillSectionHeaders).ToArray());

            result.EducationSection = ExtractSection(text,
                EducationSectionHeaders, AllSectionHeaders.Except(EducationSectionHeaders).ToArray());

            result.ExperienceSection = ExtractSection(text,
                ExperienceSectionHeaders, AllSectionHeaders.Except(ExperienceSectionHeaders).ToArray());

            var summarySection = ExtractSection(text,
                SummarySectionHeaders, AllSectionHeaders.Except(SummarySectionHeaders).ToArray());
            result.Summary = string.IsNullOrWhiteSpace(summarySection)
                ? ExtractFallbackSummary(text)
                : summarySection?.Length > 600
                    ? summarySection[..600] + "..."
                    : summarySection;

            result.ExtractedSkills = ExtractSkills(
                string.IsNullOrEmpty(result.SkillsSection) ? text : result.SkillsSection + "\n" + text);

            result.ExtractedEducation = ParseEducationLines(result.EducationSection ?? text);
            result.ExtractedExperience = ParseExperienceLines(result.ExperienceSection ?? text);

            result.EstimatedExperienceYears = EstimateExperienceYears(text);
            result.HighestEducation = InferHighestEducation(text);
        }

        // ─── Field Extractors ──────────────────────────────────────────────

        public string? ExtractEmail(string text)
        {
            var match = Regex.Match(text,
                @"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}",
                RegexOptions.IgnoreCase);
            return match.Success ? match.Value.Trim() : null;
        }

        public string? ExtractPhone(string text)
        {
            var patterns = new[]
            {
                @"\+?1?\s*[\-.]?\s*\(?\d{3}\)?[\s\-.]?\d{3}[\s\-.]?\d{4}",
                @"\+\d{1,3}[\s\-]?\d{7,14}",
                @"\d{10}",
                @"\(\d{3}\)\s*\d{3}[\s\-]\d{4}"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern);
                if (match.Success)
                {
                    var phone = Regex.Replace(match.Value, @"[^\d+\-\(\)\s]", "").Trim();
                    if (phone.Length >= 7) return phone;
                }
            }
            return null;
        }

        public string? ExtractName(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            // Names typically appear in the first 3 lines
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                            .Select(l => l.Trim())
                            .Where(l => l.Length > 2 && l.Length < 60)
                            .Take(5)
                            .ToList();

            foreach (var line in lines)
            {
                // Skip lines that look like headings, emails, phones, or URLs
                if (line.Contains('@') || line.Contains("http") ||
                    Regex.IsMatch(line, @"\d{5,}") ||
                    line.ToLower().Contains("resume") ||
                    line.ToLower().Contains("curriculum") ||
                    line.ToLower().Contains("cv"))
                    continue;

                // Check if line looks like a name (2-4 capitalized words)
                var words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length >= 2 && words.Length <= 5 &&
                    words.All(w => w.Length >= 2 && char.IsUpper(w[0])))
                {
                    return string.Join(" ", words);
                }
            }

            return null;
        }

        public string ExtractSection(
            string text,
            string[] sectionHeaders,
            string[] nextSectionHeaders)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var lines = text.Split('\n').ToList();
            int startLine = -1;
            int endLine = lines.Count;

            // Find start line
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim().ToLower();
                if (sectionHeaders.Any(h =>
                    line.Equals(h, StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith(h + ":") ||
                    line.StartsWith(h + " ")))
                {
                    startLine = i + 1;
                    break;
                }
            }

            if (startLine == -1) return string.Empty;

            // Find end line (next section header)
            for (int i = startLine; i < lines.Count; i++)
            {
                var line = lines[i].Trim().ToLower();
                if (nextSectionHeaders.Any(h =>
                    line.Equals(h, StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith(h + ":") ||
                    (line.Length < 40 && line.StartsWith(h))))
                {
                    endLine = i;
                    break;
                }
            }

            var sectionLines = lines
                .Skip(startLine)
                .Take(endLine - startLine)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            return string.Join("\n", sectionLines);
        }

        // ─── Skill Extraction ──────────────────────────────────────────────

        public List<string> ExtractSkills(string rawText)
        {
            var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(rawText))
                return new List<string>();

            // Match against known skills
            foreach (var skill in KnownSkills)
            {
                // Word-boundary match (handle special chars in skill names)
                var escaped = Regex.Escape(skill);
                var pattern = $@"(?<![a-zA-Z]){escaped}(?![a-zA-Z])";
                if (Regex.IsMatch(rawText, pattern, RegexOptions.IgnoreCase))
                    found.Add(skill);
            }

            // Also extract comma-separated / bullet-listed skills from skills section
            var skillTokens = rawText
                .Split(new[] { ',', '|', '•', '·', '/', '\n', '\r' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length >= 2 && s.Length <= 50 &&
                            !s.Contains(' ', StringComparison.Ordinal) ||
                            s.Split(' ').Length <= 4)
                .ToList();

            foreach (var token in skillTokens)
            {
                var cleaned = Regex.Replace(token, @"[^a-zA-Z0-9#\.\+\-\s]", "").Trim();
                if (cleaned.Length >= 2 && KnownSkills.Contains(cleaned))
                    found.Add(cleaned);
            }

            return found.OrderBy(s => s).ToList();
        }

        // ─── Education Parser ──────────────────────────────────────────────

        private static List<string> ParseEducationLines(string educationSection)
        {
            if (string.IsNullOrWhiteSpace(educationSection))
                return new List<string>();

            return educationSection
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Length > 5)
                .Take(10)
                .ToList();
        }

        // ─── Experience Parser ─────────────────────────────────────────────

        private static List<string> ParseExperienceLines(string experienceSection)
        {
            if (string.IsNullOrWhiteSpace(experienceSection))
                return new List<string>();

            // Extract lines that look like job titles or company names
            return experienceSection
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Length > 5 && l.Length < 200)
                .Take(15)
                .ToList();
        }

        // ─── Experience Year Estimator ─────────────────────────────────────

        public int EstimateExperienceYears(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;

            // Pattern: "X years of experience" or "X+ years"
            var explicitMatch = Regex.Match(text,
                @"(\d{1,2})\+?\s*(?:years?|yrs?)(?:\s+of\s+(?:experience|work))?",
                RegexOptions.IgnoreCase);

            if (explicitMatch.Success &&
                int.TryParse(explicitMatch.Groups[1].Value, out int yrs) &&
                yrs is >= 0 and <= 50)
                return yrs;

            // Count date ranges like "2018 - 2023" or "Jan 2019 – Present"
            var yearRanges = Regex.Matches(text,
                @"(?:jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)?\s*(\d{4})\s*[-–—to]+\s*(present|\d{4})",
                RegexOptions.IgnoreCase);

            int totalMonths = 0;
            int currentYear = DateTime.Now.Year;

            foreach (Match range in yearRanges)
            {
                if (!int.TryParse(range.Groups[1].Value, out int startYear)) continue;
                int endYear = range.Groups[2].Value.ToLower() == "present"
                    ? currentYear
                    : int.TryParse(range.Groups[2].Value, out int ey) ? ey : currentYear;

                if (startYear >= 1990 && endYear <= currentYear + 1 && endYear >= startYear)
                    totalMonths += (endYear - startYear) * 12;
            }

            return totalMonths > 0
                ? Math.Min(totalMonths / 12, 40)
                : 0;
        }

        // ─── Education Level Inferrer ──────────────────────────────────────

        public string? InferHighestEducation(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var lowerText = text.ToLower();

            if (lowerText.Contains("ph.d") || lowerText.Contains("phd") ||
                lowerText.Contains("doctor"))
                return "PhD / Doctorate";

            if (lowerText.Contains("master") || lowerText.Contains("m.sc") ||
                lowerText.Contains("m.s.") || lowerText.Contains("mba") ||
                lowerText.Contains("m.tech") || lowerText.Contains("m.e."))
                return "Master's Degree";

            if (lowerText.Contains("bachelor") || lowerText.Contains("b.sc") ||
                lowerText.Contains("b.s.") || lowerText.Contains("b.tech") ||
                lowerText.Contains("b.e.") || lowerText.Contains("undergraduate"))
                return "Bachelor's Degree";

            if (lowerText.Contains("diploma") || lowerText.Contains("associate"))
                return "Diploma";

            if (lowerText.Contains("high school") || lowerText.Contains("secondary school") ||
                lowerText.Contains("matric") || lowerText.Contains("hsc") ||
                lowerText.Contains("ssc"))
                return "High School";

            return null;
        }

        // ─── Fallback Summary ──────────────────────────────────────────────

        private static string? ExtractFallbackSummary(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                            .Select(l => l.Trim())
                            .Where(l => l.Length > 30)
                            .Skip(3)
                            .Take(3)
                            .ToList();

            return lines.Any() ? string.Join(" ", lines) : null;
        }
    }
}