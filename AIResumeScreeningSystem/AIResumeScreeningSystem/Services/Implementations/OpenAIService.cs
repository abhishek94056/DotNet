using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.DTOs.OpenAI;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.AI;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIService> _logger;

        private readonly string _apiKey;
        private readonly string _model;
        private readonly int _maxTokens;
        private readonly double _temperature;
        private const string ApiEndpoint = "https://api.anthropic.com/v1/messages";

        public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey) &&
                                    _apiKey != "YOUR_OPENAI_API_KEY_HERE";

        public OpenAIService(
            IHttpClientFactory httpClientFactory,
            AppDbContext context,
            IConfiguration configuration,
            ILogger<OpenAIService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("OpenAI");
            _context = context;
            _configuration = configuration;
            _logger = logger;

            _apiKey = _configuration["OpenAI:ApiKey"] ?? string.Empty;
            _model = _configuration["OpenAI:Model"] ?? "claude-sonnet-4-6";
            _maxTokens = _configuration.GetValue<int>("OpenAI:MaxTokens", 1500);
            _temperature = _configuration.GetValue<double>("OpenAI:Temperature", 0.7);

            if (!string.IsNullOrEmpty(_apiKey) && _apiKey != "YOUR_OPENAI_API_KEY_HERE")
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
                _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
            }
        }

        // ─── Core API Call ─────────────────────────────────────────────────

        public async Task<string?> GetCompletionAsync(
            string systemPrompt,
            string userPrompt,
            int maxTokens = 1500,
            double temperature = 0.7)
        {
            if (!IsConfigured)
            {
                _logger.LogWarning("OpenAI API key not configured.");
                return null;
            }

            try
            {
                var requestBody = new
                {
                    model = _model,
                    max_tokens = maxTokens,
                    system = systemPrompt,
                    messages = new[]
                    {
                        new { role = "user", content = userPrompt }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ApiEndpoint, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "OpenAI API error {StatusCode}: {Body}",
                        response.StatusCode, responseBody);
                    return null;
                }

                using var doc = JsonDocument.Parse(responseBody);
                var text = doc.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString();

                return text?.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling OpenAI API");
                return null;
            }
        }

        public async Task<string?> GetChatCompletionAsync(
            List<ChatMessage> history,
            string systemPrompt,
            int maxTokens = 1000)
        {
            if (!IsConfigured) return null;

            try
            {
                var messages = history.Select(h => new
                {
                    role = h.Role.ToLower(),
                    content = h.Content
                }).ToList();

                var requestBody = new
                {
                    model = _model,
                    max_tokens = maxTokens,
                    system = systemPrompt,
                    messages
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ApiEndpoint, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Chat API error: {Body}", responseBody);
                    return null;
                }

                using var doc = JsonDocument.Parse(responseBody);
                return doc.RootElement
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetString()?.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in chat completion");
                return null;
            }
        }

        // ─── Resume Summary ────────────────────────────────────────────────

        public async Task<ResumeSummaryViewModel> GenerateResumeSummaryAsync(int resumeId)
        {
            var result = new ResumeSummaryViewModel { ResumeId = resumeId };

            try
            {
                var resume = await _context.Resumes
                    .Include(r => r.Candidate).ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(r => r.Id == resumeId);

                if (resume == null)
                {
                    result.Error = "Resume not found.";
                    return result;
                }

                result.CandidateId = resume.CandidateId;
                result.CandidateName =
                    $"{resume.Candidate.User.FirstName} {resume.Candidate.User.LastName}";
                result.FileName = resume.FileName;

                var resumeText = BuildResumeContext(resume);

                if (!IsConfigured)
                {
                    PopulateFallbackSummary(result, resume);
                    result.IsLoaded = true;
                    return result;
                }

                var systemPrompt = @"You are an expert HR analyst and career coach. 
Analyze the provided resume and return a structured JSON response.
Always respond with valid JSON only — no markdown, no extra text.";

                var userPrompt = $@"Analyze this resume and provide a comprehensive evaluation.

RESUME:
{resumeText}

Return ONLY this JSON structure:
{{
  ""executiveSummary"": ""2-3 sentence professional overview"",
  ""technicalProfile"": ""Technical skills assessment paragraph"",
  ""careerHighlights"": ""Key career achievements paragraph"",
  ""educationSummary"": ""Education background paragraph"",
  ""improvementSuggestions"": ""Specific resume improvement advice"",
  ""topSkills"": [""skill1"", ""skill2"", ""skill3"", ""skill4"", ""skill5""],
  ""achievements"": [""achievement1"", ""achievement2"", ""achievement3""],
  ""improvementTips"": [""tip1"", ""tip2"", ""tip3""],
  ""resumeQualityScore"": 75,
  ""contentCompletenessScore"": 80,
  ""presentationScore"": 70
}}";

                var rawResponse = await GetCompletionAsync(
                    systemPrompt, userPrompt, 2000, 0.5);

                if (!string.IsNullOrEmpty(rawResponse))
                {
                    ParseResumeSummaryResponse(result, rawResponse);
                    result.IsLoaded = true;

                    // Cache summary in resume record
                    if (!string.IsNullOrEmpty(result.ExecutiveSummary))
                    {
                        resume.ParsedSummary = result.ExecutiveSummary;
                        _context.Resumes.Update(resume);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    PopulateFallbackSummary(result, resume);
                    result.IsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating resume summary for {ResumeId}", resumeId);
                result.Error = "An error occurred generating the summary.";
            }

            return result;
        }

        public async Task<string?> GenerateResumeSummaryTextAsync(
            string resumeText, string candidateName)
        {
            var systemPrompt = "You are a professional resume writer. Create a concise, " +
                               "impactful 3-sentence professional summary.";
            var userPrompt = $"Write a professional summary for {candidateName} " +
                             $"based on this resume:\n\n{resumeText[..Math.Min(3000, resumeText.Length)]}";

            return await GetCompletionAsync(systemPrompt, userPrompt, 300, 0.6);
        }

        // ─── Candidate Evaluation ──────────────────────────────────────────

        public async Task<AIEvaluationViewModel> EvaluateCandidateAsync(int applicationId)
        {
            var result = new AIEvaluationViewModel { ApplicationId = applicationId };

            try
            {
                var application = await _context.Applications
                    .Include(a => a.Job)
                        .ThenInclude(j => j.JobSkills).ThenInclude(js => js.Skill)
                    .Include(a => a.Candidate)
                        .ThenInclude(c => c.User)
                    .Include(a => a.Candidate)
                        .ThenInclude(c => c.CandidateSkills).ThenInclude(cs => cs.Skill)
                    .Include(a => a.Resume)
                    .FirstOrDefaultAsync(a => a.Id == applicationId);

                if (application == null)
                {
                    result.Error = "Application not found.";
                    return result;
                }

                result.JobId = application.JobId;
                result.CandidateId = application.CandidateId;
                result.JobTitle = application.Job?.Title ?? string.Empty;
                result.Company = application.Job?.Company ?? string.Empty;
                result.CandidateName =
                    $"{application.Candidate.User.FirstName} {application.Candidate.User.LastName}";
                result.OverallMatchScore = application.AIMatchScore ?? 0;

                if (!IsConfigured)
                {
                    PopulateFallbackEvaluation(result, application);
                    result.IsLoaded = true;
                    return result;
                }

                var jobSkills = application.Job?.JobSkills
                    .Select(js => $"{js.Skill.Name} ({(js.IsRequired ? "Required" : "Optional")})")
                    .ToList() ?? new();

                var candidateSkills = application.Candidate.CandidateSkills
                    .Select(cs => $"{cs.Skill.Name} ({cs.ProficiencyLevel})")
                    .ToList();

                var systemPrompt = @"You are a senior technical recruiter with 15 years of experience.
Evaluate the candidate objectively and return valid JSON only.";

                var userPrompt = $@"Evaluate this candidate for the job position.

JOB: {application.Job?.Title} at {application.Job?.Company}
JOB DESCRIPTION: {application.Job?.Description?[..Math.Min(500, application.Job.Description.Length)]}
REQUIRED SKILLS: {string.Join(", ", jobSkills)}
EXPERIENCE REQUIRED: {application.Job?.ExperienceYearsMin}-{application.Job?.ExperienceYearsMax} years

CANDIDATE: {result.CandidateName}
EXPERIENCE: {application.Candidate.TotalExperienceYears} years
EDUCATION: {application.Candidate.HighestEducation}
SKILLS: {string.Join(", ", candidateSkills)}
CURRENT ROLE: {application.Candidate.CurrentJobTitle} at {application.Candidate.CurrentCompany}
HEADLINE: {application.Candidate.Headline}
AI MATCH SCORE: {result.OverallMatchScore}%

Return ONLY this JSON:
{{
  ""evaluationSummary"": ""Comprehensive 3-4 sentence evaluation"",
  ""strengthsAnalysis"": ""Detailed strengths paragraph"",
  ""weaknessesAnalysis"": ""Areas for improvement paragraph"",
  ""hiringRecommendation"": ""Clear hire/no-hire recommendation with reasoning"",
  ""culturalFitAssessment"": ""Cultural fit notes"",
  ""careerProgressionNote"": ""Career trajectory assessment"",
  ""keyStrengths"": [""strength1"", ""strength2"", ""strength3""],
  ""keyWeaknesses"": [""weakness1"", ""weakness2""],
  ""redFlags"": [],
  ""recommendationLevel"": ""Strongly Recommend | Recommend | Consider | Not Recommended""
}}";

                var rawResponse = await GetCompletionAsync(systemPrompt, userPrompt, 2000, 0.6);

                if (!string.IsNullOrEmpty(rawResponse))
                {
                    ParseEvaluationResponse(result, rawResponse);
                    result.IsLoaded = true;

                    // Persist evaluation to application record
                    application.AIEvaluation = result.EvaluationSummary;
                    _context.Applications.Update(application);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    PopulateFallbackEvaluation(result, application);
                    result.IsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating candidate for application {AppId}", applicationId);
                result.Error = "An error occurred during evaluation.";
            }

            return result;
        }

        // ─── Skill Gap Analysis ────────────────────────────────────────────

        public async Task<SkillGapAIViewModel> GenerateSkillGapAnalysisAsync(int applicationId)
        {
            var result = new SkillGapAIViewModel { ApplicationId = applicationId };

            try
            {
                var application = await _context.Applications
                    .Include(a => a.Job)
                        .ThenInclude(j => j.JobSkills).ThenInclude(js => js.Skill)
                    .Include(a => a.Candidate)
                        .ThenInclude(c => c.User)
                    .Include(a => a.Candidate)
                        .ThenInclude(c => c.CandidateSkills).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(a => a.Id == applicationId);

                if (application == null)
                {
                    result.Error = "Application not found.";
                    return result;
                }

                result.JobId = application.JobId;
                result.CandidateId = application.CandidateId;
                result.JobTitle = application.Job?.Title ?? string.Empty;
                result.CandidateName =
                    $"{application.Candidate.User.FirstName} {application.Candidate.User.LastName}";
                result.MatchScore = application.AIMatchScore ?? 0;

                var candidateSkillNames = application.Candidate.CandidateSkills
                    .Select(cs => cs.Skill.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

                result.MissingRequiredSkills = application.Job?.JobSkills
                    .Where(js => js.IsRequired &&
                                 !candidateSkillNames.Contains(js.Skill.Name))
                    .Select(js => js.Skill.Name).ToList() ?? new();

                result.MissingOptionalSkills = application.Job?.JobSkills
                    .Where(js => !js.IsRequired &&
                                 !candidateSkillNames.Contains(js.Skill.Name))
                    .Select(js => js.Skill.Name).ToList() ?? new();

                if (!IsConfigured)
                {
                    PopulateFallbackSkillGap(result);
                    result.IsLoaded = true;
                    return result;
                }

                if (!result.MissingRequiredSkills.Any() && !result.MissingOptionalSkills.Any())
                {
                    result.GapAnalysisNarrative =
                        "Excellent! This candidate meets all skill requirements for the position.";
                    result.TimeToReadiness = "Ready Now";
                    result.PriorityRecommendation =
                        "No skill gaps identified. Candidate is fully qualified.";
                    result.IsLoaded = true;
                    return result;
                }

                var systemPrompt = @"You are a technical skills coach and career development expert.
Provide actionable learning guidance. Return valid JSON only.";

                var userPrompt = $@"Create a personalized skill gap analysis and learning plan.

CANDIDATE: {result.CandidateName}
APPLYING FOR: {result.JobTitle}
CURRENT SKILLS: {string.Join(", ", candidateSkillNames)}
MISSING REQUIRED SKILLS: {string.Join(", ", result.MissingRequiredSkills)}
MISSING OPTIONAL SKILLS: {string.Join(", ", result.MissingOptionalSkills)}
CURRENT MATCH SCORE: {result.MatchScore}%

Return ONLY this JSON:
{{
  ""gapAnalysisNarrative"": ""Detailed analysis of the skill gaps and their impact"",
  ""learningRoadmap"": ""Step-by-step learning path description"",
  ""timeToReadiness"": ""e.g. 3-6 months with dedicated study"",
  ""priorityRecommendation"": ""Which skill to learn first and why"",
  ""learningPlan"": [
    {{
      ""skillName"": ""Skill Name"",
      ""priority"": ""Critical | High | Medium | Low"",
      ""estimatedTime"": ""e.g. 4 weeks"",
      ""resourceType"": ""Online Course | Book | Practice Project | Certification"",
      ""courseName"": ""Recommended resource name"",
      ""resourceUrl"": ""https://..."" 
    }}
  ]
}}";

                var rawResponse = await GetCompletionAsync(systemPrompt, userPrompt, 2500, 0.5);

                if (!string.IsNullOrEmpty(rawResponse))
                {
                    ParseSkillGapResponse(result, rawResponse);
                    result.IsLoaded = true;

                    // Persist to application
                    application.SkillGapAnalysis = result.GapAnalysisNarrative;
                    _context.Applications.Update(application);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    PopulateFallbackSkillGap(result);
                    result.IsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating skill gap for application {AppId}", applicationId);
                result.Error = "An error occurred generating the skill gap analysis.";
            }

            return result;
        }

        // ─── Interview Questions ───────────────────────────────────────────

        public async Task<InterviewQuestionsViewModel> GenerateInterviewQuestionsAsync(
            int jobId,
            int? applicationId = null,
            int count = 10)
        {
            var result = new InterviewQuestionsViewModel
            {
                JobId = jobId,
                ApplicationId = applicationId
            };

            try
            {
                var job = await _context.Jobs
                    .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                    .FirstOrDefaultAsync(j => j.Id == jobId);

                if (job == null)
                {
                    result.Error = "Job not found.";
                    return result;
                }

                result.JobTitle = job.Title;
                result.Company = job.Company;

                // Load existing saved questions
                result.SavedQuestions = await _context.InterviewQuestions
                    .Where(q => q.JobId == jobId)
                    .OrderBy(q => q.GeneratedAt)
                    .ToListAsync();

                // Load candidate context if application provided
                string candidateContext = string.Empty;
                if (applicationId.HasValue)
                {
                    var application = await _context.Applications
                        .Include(a => a.Candidate)
                            .ThenInclude(c => c.User)
                        .Include(a => a.Candidate)
                            .ThenInclude(c => c.CandidateSkills).ThenInclude(cs => cs.Skill)
                        .FirstOrDefaultAsync(a => a.Id == applicationId.Value);

                    if (application != null)
                    {
                        result.CandidateId = application.CandidateId;
                        result.CandidateName =
                            $"{application.Candidate.User.FirstName} {application.Candidate.User.LastName}";
                        var skills = application.Candidate.CandidateSkills
                            .Select(cs => cs.Skill.Name).ToList();
                        candidateContext =
                            $"\nCANDIDATE SKILLS: {string.Join(", ", skills)}" +
                            $"\nCANDIDATE EXPERIENCE: {application.Candidate.TotalExperienceYears} years";
                    }
                }

                var requiredSkills = job.JobSkills
                    .Where(js => js.IsRequired)
                    .Select(js => js.Skill.Name).ToList();

                if (!IsConfigured)
                {
                    result.Questions = GenerateFallbackQuestions(job.Title, requiredSkills);
                    result.IsLoaded = true;
                    return result;
                }

                var systemPrompt = @"You are a senior technical interviewer with expertise in hiring.
Generate insightful interview questions. Return valid JSON only.";

                var userPrompt = $@"Generate {count} interview questions for this position.

JOB: {job.Title} at {job.Company}
DESCRIPTION: {job.Description?[..Math.Min(400, job.Description.Length)]}
REQUIRED SKILLS: {string.Join(", ", requiredSkills)}
EXPERIENCE: {job.ExperienceYearsMin}-{job.ExperienceYearsMax} years
{candidateContext}

Mix of: Technical (40%), Behavioral (30%), Situational (20%), Cultural Fit (10%).
Vary difficulty: Easy (20%), Medium (50%), Hard (30%).

Return ONLY this JSON:
{{
  ""questions"": [
    {{
      ""question"": ""The interview question"",
      ""expectedAnswer"": ""Key points to look for in a good answer"",
      ""category"": ""Technical | Behavioral | Situational | Cultural Fit"",
      ""difficulty"": ""Easy | Medium | Hard""
    }}
  ]
}}";

                var rawResponse = await GetCompletionAsync(systemPrompt, userPrompt, 3000, 0.8);

                if (!string.IsNullOrEmpty(rawResponse))
                {
                    ParseInterviewQuestionsResponse(result, rawResponse);

                    // Save generated questions to database
                    foreach (var q in result.Questions)
                    {
                        var dbQuestion = new InterviewQuestion
                        {
                            JobId = jobId,
                            ApplicationId = applicationId,
                            Question = q.Question,
                            ExpectedAnswer = q.ExpectedAnswer,
                            QuestionType = q.Category.ToLower() switch
                            {
                                "behavioral" => QuestionType.Behavioral,
                                "situational" => QuestionType.Situational,
                                "cultural fit" => QuestionType.CulturalFit,
                                _ => QuestionType.Technical
                            },
                            DifficultyLevel = q.Difficulty.ToLower() switch
                            {
                                "easy" => DifficultyLevel.Easy,
                                "hard" => DifficultyLevel.Hard,
                                _ => DifficultyLevel.Medium
                            },
                            IsAIGenerated = true,
                            GeneratedAt = DateTime.UtcNow
                        };
                        await _context.InterviewQuestions.AddAsync(dbQuestion);
                    }
                    await _context.SaveChangesAsync();
                    result.IsLoaded = true;
                }
                else
                {
                    result.Questions = GenerateFallbackQuestions(job.Title, requiredSkills);
                    result.IsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generating interview questions for Job {JobId}", jobId);
                result.Error = "An error occurred generating interview questions.";
            }

            return result;
        }

        // ─── Candidate Recommendations ─────────────────────────────────────

        public async Task<AIRecommendationViewModel> GenerateCandidateRecommendationsAsync(
            int candidateId)
        {
            var result = new AIRecommendationViewModel { CandidateId = candidateId };

            try
            {
                var candidate = await _context.Candidates
                    .Include(c => c.User)
                    .Include(c => c.CandidateSkills).ThenInclude(cs => cs.Skill)
                    .Include(c => c.Applications).ThenInclude(a => a.Job)
                    .FirstOrDefaultAsync(c => c.Id == candidateId);

                if (candidate == null)
                {
                    result.Error = "Candidate not found.";
                    return result;
                }

                result.CandidateName =
                    $"{candidate.User.FirstName} {candidate.User.LastName}";
                result.CandidateSkills = candidate.CandidateSkills
                    .Select(cs => cs.Skill.Name).ToList();

                if (!IsConfigured)
                {
                    PopulateFallbackRecommendations(result);
                    result.IsLoaded = true;
                    return result;
                }

                var appliedJobs = candidate.Applications
                    .Select(a => a.Job?.Title)
                    .Where(t => t != null).ToList();

                var systemPrompt = @"You are a career counselor and job market expert.
Provide personalized career guidance. Return valid JSON only.";

                var userPrompt = $@"Provide career recommendations for this candidate.

CANDIDATE: {result.CandidateName}
EXPERIENCE: {candidate.TotalExperienceYears} years
EDUCATION: {candidate.HighestEducation}
CURRENT ROLE: {candidate.CurrentJobTitle} at {candidate.CurrentCompany}
SKILLS: {string.Join(", ", result.CandidateSkills)}
RECENTLY APPLIED TO: {string.Join(", ", appliedJobs.Take(5))}
HEADLINE: {candidate.Headline}

Return ONLY this JSON:
{{
  ""careerAdvice"": ""Personalized career development advice paragraph"",
  ""jobSearchStrategy"": ""Specific job search recommendations"",
  ""skillDevelopmentPlan"": ""Skills to develop next for career growth"",
  ""industryInsights"": ""Market trends relevant to this candidate"",
  ""recommendedRoles"": [
    {{
      ""roleTitle"": ""Job Title"",
      ""reasoning"": ""Why this role suits the candidate"",
      ""matchConfidence"": 85,
      ""industry"": ""Industry name"",
      ""keySkillsNeeded"": [""skill1"", ""skill2""]
    }}
  ]
}}";

                var rawResponse = await GetCompletionAsync(systemPrompt, userPrompt, 2500, 0.7);

                if (!string.IsNullOrEmpty(rawResponse))
                {
                    ParseRecommendationsResponse(result, rawResponse);
                    result.IsLoaded = true;
                }
                else
                {
                    PopulateFallbackRecommendations(result);
                    result.IsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generating recommendations for candidate {CandidateId}", candidateId);
                result.Error = "An error occurred generating recommendations.";
            }

            return result;
        }

        // ─── Chatbot ───────────────────────────────────────────────────────

        public async Task<string> GetChatbotResponseAsync(
            string userMessage,
            List<ChatMessage> history,
            string? context = null)
        {
            if (!IsConfigured)
                return "AI Chatbot is not configured. Please add your OpenAI API key in appsettings.json.";

            try
            {
                var systemPrompt = $@"You are an intelligent AI Recruiter Assistant for the 
AI Resume Screening System. You help recruiters and candidates with:
- Understanding match scores and skill gaps
- Resume improvement advice
- Interview preparation tips  
- Job search strategies
- Platform navigation help

Be concise, professional, and helpful. {(string.IsNullOrEmpty(context) ? "" : $"\nCONTEXT: {context}")}";

                var messages = history
                    .TakeLast(10) // Keep last 10 messages for context
                    .ToList();

                messages.Add(new ChatMessage { Role = "user", Content = userMessage });

                var response = await GetChatCompletionAsync(messages, systemPrompt, 800);

                return response ?? "I'm sorry, I couldn't generate a response. Please try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chatbot error");
                return "An error occurred. Please try again.";
            }
        }

        // ─── Quick Helpers ─────────────────────────────────────────────────

        public async Task<string?> ImproveResumeTextAsync(string resumeText)
        {
            var systemPrompt = "You are a professional resume writer. Improve the resume content while keeping it truthful. Return improved text only.";
            var userPrompt = $"Improve this resume section:\n\n{resumeText[..Math.Min(2000, resumeText.Length)]}";
            return await GetCompletionAsync(systemPrompt, userPrompt, 1000, 0.6);
        }

        public async Task<decimal> PredictApplicationSuccessAsync(int applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.Job)
                .Include(a => a.Candidate)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null) return 0;

            // Composite success prediction based on available data
            decimal score = application.AIMatchScore ?? 0;
            decimal experienceBonus = Math.Min(application.Candidate.TotalExperienceYears * 2, 15);
            decimal educationBonus = application.Candidate.HighestEducation?.ToLower()
                .Contains("master") == true ? 5 : 0;

            return Math.Min(score * 0.7m + experienceBonus + educationBonus, 95);
        }

        // ─── JSON Parsers ──────────────────────────────────────────────────

        private static void ParseResumeSummaryResponse(
            ResumeSummaryViewModel result, string rawJson)
        {
            try
            {
                var clean = CleanJsonResponse(rawJson);
                using var doc = JsonDocument.Parse(clean);
                var root = doc.RootElement;

                result.ExecutiveSummary = TryGetString(root, "executiveSummary");
                result.TechnicalProfile = TryGetString(root, "technicalProfile");
                result.CareerHighlights = TryGetString(root, "careerHighlights");
                result.EducationSummary = TryGetString(root, "educationSummary");
                result.ImprovementSuggestions = TryGetString(root, "improvementSuggestions");
                result.TopSkills = TryGetStringList(root, "topSkills");
                result.Achievements = TryGetStringList(root, "achievements");
                result.ImprovementTips = TryGetStringList(root, "improvementTips");
                result.ResumeQualityScore = TryGetInt(root, "resumeQualityScore", 70);
                result.ContentCompletenessScore = TryGetInt(root, "contentCompletenessScore", 70);
                result.PresentationScore = TryGetInt(root, "presentationScore", 70);
            }
            catch
            {
                result.ExecutiveSummary = ExtractTextFallback(rawJson);
            }
        }

        private static void ParseEvaluationResponse(
            AIEvaluationViewModel result, string rawJson)
        {
            try
            {
                var clean = CleanJsonResponse(rawJson);
                using var doc = JsonDocument.Parse(clean);
                var root = doc.RootElement;

                result.EvaluationSummary = TryGetString(root, "evaluationSummary");
                result.StrengthsAnalysis = TryGetString(root, "strengthsAnalysis");
                result.WeaknessesAnalysis = TryGetString(root, "weaknessesAnalysis");
                result.HiringRecommendation = TryGetString(root, "hiringRecommendation");
                result.CulturalFitAssessment = TryGetString(root, "culturalFitAssessment");
                result.CareerProgressionNote = TryGetString(root, "careerProgressionNote");
                result.KeyStrengths = TryGetStringList(root, "keyStrengths");
                result.KeyWeaknesses = TryGetStringList(root, "keyWeaknesses");
                result.RedFlags = TryGetStringList(root, "redFlags");
                result.RecommendationLevel = TryGetString(root, "recommendationLevel")
                    ?? "Consider";
            }
            catch
            {
                result.EvaluationSummary = ExtractTextFallback(rawJson);
                result.RecommendationLevel = "Consider";
            }
        }

        private static void ParseSkillGapResponse(
            SkillGapAIViewModel result, string rawJson)
        {
            try
            {
                var clean = CleanJsonResponse(rawJson);
                using var doc = JsonDocument.Parse(clean);
                var root = doc.RootElement;

                result.GapAnalysisNarrative = TryGetString(root, "gapAnalysisNarrative");
                result.LearningRoadmap = TryGetString(root, "learningRoadmap");
                result.TimeToReadiness = TryGetString(root, "timeToReadiness");
                result.PriorityRecommendation = TryGetString(root, "priorityRecommendation");

                if (root.TryGetProperty("learningPlan", out var planArray) &&
                    planArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in planArray.EnumerateArray())
                    {
                        result.LearningPlan.Add(new SkillLearningItem
                        {
                            SkillName = TryGetString(item, "skillName") ?? string.Empty,
                            Priority = TryGetString(item, "priority") ?? "Medium",
                            EstimatedTime = TryGetString(item, "estimatedTime") ?? "TBD",
                            ResourceType = TryGetString(item, "resourceType") ?? "Online Course",
                            CourseName = TryGetString(item, "courseName"),
                            ResourceUrl = TryGetString(item, "resourceUrl")
                        });
                    }
                }
            }
            catch
            {
                result.GapAnalysisNarrative = ExtractTextFallback(rawJson);
            }
        }

        private static void ParseInterviewQuestionsResponse(
            InterviewQuestionsViewModel result, string rawJson)
        {
            try
            {
                var clean = CleanJsonResponse(rawJson);
                using var doc = JsonDocument.Parse(clean);
                var root = doc.RootElement;

                if (root.TryGetProperty("questions", out var questionsArray) &&
                    questionsArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in questionsArray.EnumerateArray())
                    {
                        result.Questions.Add(new GeneratedQuestion
                        {
                            Question = TryGetString(item, "question") ?? string.Empty,
                            ExpectedAnswer = TryGetString(item, "expectedAnswer"),
                            Category = TryGetString(item, "category") ?? "Technical",
                            Difficulty = TryGetString(item, "difficulty") ?? "Medium"
                        });
                    }
                }
            }
            catch
            {
                // If JSON parse fails, generate fallbacks
                result.Questions = GenerateFallbackQuestions(result.JobTitle, new List<string>());
            }
        }

        private static void ParseRecommendationsResponse(
            AIRecommendationViewModel result, string rawJson)
        {
            try
            {
                var clean = CleanJsonResponse(rawJson);
                using var doc = JsonDocument.Parse(clean);
                var root = doc.RootElement;

                result.CareerAdvice = TryGetString(root, "careerAdvice");
                result.JobSearchStrategy = TryGetString(root, "jobSearchStrategy");
                result.SkillDevelopmentPlan = TryGetString(root, "skillDevelopmentPlan");
                result.IndustryInsights = TryGetString(root, "industryInsights");

                if (root.TryGetProperty("recommendedRoles", out var rolesArray) &&
                    rolesArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in rolesArray.EnumerateArray())
                    {
                        result.RecommendedRoles.Add(new RecommendedRoleItem
                        {
                            RoleTitle = TryGetString(item, "roleTitle") ?? string.Empty,
                            Reasoning = TryGetString(item, "reasoning") ?? string.Empty,
                            MatchConfidence = TryGetInt(item, "matchConfidence", 70),
                            Industry = TryGetString(item, "industry") ?? string.Empty,
                            KeySkillsNeeded = TryGetStringList(item, "keySkillsNeeded")
                        });
                    }
                }
            }
            catch
            {
                PopulateFallbackRecommendations(result);
            }
        }

        // ─── Fallbacks (when AI is not configured) ─────────────────────────

        private static void PopulateFallbackSummary(
            ResumeSummaryViewModel result, Resume resume)
        {
            result.ExecutiveSummary = resume.ParsedSummary
                ?? "Professional resume on file. Enable AI features for a detailed summary.";
            result.TechnicalProfile =
                "Technical skills extracted from resume. Enable AI for deeper analysis.";
            result.TopSkills = resume.ParsedSkills?
                .Split(',').Select(s => s.Trim()).Take(5).ToList() ?? new();
            result.ResumeQualityScore = 65;
            result.ContentCompletenessScore = 70;
            result.PresentationScore = 60;
            result.ImprovementSuggestions =
                "Configure your OpenAI API key in appsettings.json for AI-powered suggestions.";
        }

        private static void PopulateFallbackEvaluation(
            AIEvaluationViewModel result, Application application)
        {
            result.EvaluationSummary =
                $"{result.CandidateName} has applied for {result.JobTitle}. " +
                $"AI match score: {result.OverallMatchScore}%. " +
                "Configure OpenAI API key for a detailed AI evaluation.";
            result.RecommendationLevel = result.OverallMatchScore >= 70
                ? "Recommend" : result.OverallMatchScore >= 50 ? "Consider" : "Not Recommended";
            result.KeyStrengths = new List<string>
                { "Profile submitted", "Resume uploaded" };
            result.KeyWeaknesses = new List<string>
                { "AI evaluation requires API key configuration" };
        }

        private static void PopulateFallbackSkillGap(SkillGapAIViewModel result)
        {
            result.GapAnalysisNarrative =
                $"The candidate is missing {result.MissingRequiredSkills.Count} required skills. " +
                "Configure OpenAI API key for a detailed learning roadmap.";
            result.TimeToReadiness = "Varies by skill";
            result.PriorityRecommendation =
                result.MissingRequiredSkills.FirstOrDefault()
                ?? "Review required skills and update profile.";
        }

        private static void PopulateFallbackRecommendations(AIRecommendationViewModel result)
        {
            result.CareerAdvice =
                "Continue building your skill profile to improve match scores. " +
                "Configure OpenAI API key for personalized AI career advice.";
            result.JobSearchStrategy =
                "Apply to jobs that match at least 60% of your skills. " +
                "Focus on roles where you meet the core requirements.";
        }

        private static List<GeneratedQuestion> GenerateFallbackQuestions(
            string jobTitle, List<string> skills)
        {
            var questions = new List<GeneratedQuestion>
            {
                new() {
                    Question = $"Tell me about your experience relevant to {jobTitle}.",
                    ExpectedAnswer = "Look for specific examples and measurable outcomes.",
                    Category = "Behavioral", Difficulty = "Easy"
                },
                new() {
                    Question = "Describe a challenging technical problem you solved recently.",
                    ExpectedAnswer = "STAR method: Situation, Task, Action, Result.",
                    Category = "Behavioral", Difficulty = "Medium"
                },
                new() {
                    Question = "How do you stay current with industry trends and technologies?",
                    ExpectedAnswer = "Look for continuous learning mindset, specific resources.",
                    Category = "Cultural Fit", Difficulty = "Easy"
                },
                new() {
                    Question = "Where do you see yourself professionally in 5 years?",
                    ExpectedAnswer = "Alignment with role growth, ambition, company fit.",
                    Category = "Cultural Fit", Difficulty = "Easy"
                }
            };

            foreach (var skill in skills.Take(3))
            {
                questions.Add(new GeneratedQuestion
                {
                    Question = $"Rate your proficiency in {skill} and describe a project where you used it.",
                    ExpectedAnswer = $"Should demonstrate hands-on {skill} experience with concrete examples.",
                    Category = "Technical",
                    Difficulty = "Medium"
                });
            }

            return questions;
        }

        // ─── JSON Helper Utilities ─────────────────────────────────────────

        private static string CleanJsonResponse(string raw)
        {
            // Remove markdown code fences
            raw = Regex.Replace(raw, @"```json\s*", "", RegexOptions.IgnoreCase);
            raw = Regex.Replace(raw, @"```\s*", "");

            // Extract JSON object if surrounded by text
            var match = Regex.Match(raw, @"\{[\s\S]*\}", RegexOptions.Singleline);
            return match.Success ? match.Value : raw.Trim();
        }

        private static string? TryGetString(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var prop) &&
                   prop.ValueKind == JsonValueKind.String
                ? prop.GetString()
                : null;
        }

        private static int TryGetInt(JsonElement element, string propertyName, int defaultValue = 0)
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.Number)
                    return prop.GetInt32();
                if (prop.ValueKind == JsonValueKind.String &&
                    int.TryParse(prop.GetString(), out var parsed))
                    return parsed;
            }
            return defaultValue;
        }

        private static List<string> TryGetStringList(JsonElement element, string propertyName)
        {
            var list = new List<string>();
            if (element.TryGetProperty(propertyName, out var prop) &&
                prop.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in prop.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String)
                    {
                        var val = item.GetString();
                        if (!string.IsNullOrWhiteSpace(val))
                            list.Add(val);
                    }
                }
            }
            return list;
        }

        private static string BuildResumeContext(Resume resume)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(resume.ParsedName))
                sb.AppendLine($"Name: {resume.ParsedName}");
            if (!string.IsNullOrEmpty(resume.ParsedEmail))
                sb.AppendLine($"Email: {resume.ParsedEmail}");
            if (!string.IsNullOrEmpty(resume.ParsedSkills))
                sb.AppendLine($"Skills: {resume.ParsedSkills}");
            if (!string.IsNullOrEmpty(resume.ParsedEducation))
                sb.AppendLine($"Education: {resume.ParsedEducation}");
            if (!string.IsNullOrEmpty(resume.ParsedExperience))
                sb.AppendLine($"Experience: {resume.ParsedExperience}");
            if (!string.IsNullOrEmpty(resume.RawText))
                sb.AppendLine(resume.RawText[..Math.Min(3000, resume.RawText.Length)]);
            return sb.ToString();
        }

        private static string ExtractTextFallback(string rawText)
        {
            // Try to extract meaningful text from malformed JSON or plain text
            var cleaned = Regex.Replace(rawText, @"[{}""\[\]]", " ");
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
            return cleaned.Length > 500 ? cleaned[..500] + "..." : cleaned;
        }
    }
}