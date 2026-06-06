using AIResumeScreeningSystem.DTOs.OpenAI;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeScreeningSystem.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly IOpenAIService _openAIService;
        private readonly ICandidateService _candidateService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AIController> _logger;

        public AIController(
            IOpenAIService openAIService,
            ICandidateService candidateService,
            UserManager<ApplicationUser> userManager,
            ILogger<AIController> logger)
        {
            _openAIService = openAIService;
            _candidateService = candidateService;
            _userManager = userManager;
            _logger = logger;
        }

        // ─── Resume Summary ────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ResumeSummary(int resumeId)
        {
            var viewModel = await _openAIService.GenerateResumeSummaryAsync(resumeId);
            return View(viewModel);
        }

        // ─── Candidate Evaluation ──────────────────────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        [HttpGet]
        public async Task<IActionResult> Evaluate(int applicationId)
        {
            var viewModel = await _openAIService.EvaluateCandidateAsync(applicationId);
            return View(viewModel);
        }

        // ─── Skill Gap Analysis ────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> SkillGap(int applicationId)
        {
            var viewModel = await _openAIService.GenerateSkillGapAnalysisAsync(applicationId);
            return View(viewModel);
        }

        // ─── Interview Questions ───────────────────────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        [HttpGet]
        public async Task<IActionResult> InterviewQuestions(
            int jobId, int? applicationId = null, int count = 10)
        {
            var viewModel = await _openAIService.GenerateInterviewQuestionsAsync(
                jobId, applicationId, count);
            return View(viewModel);
        }

        // ─── Candidate Recommendations ─────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Recommendations(int? candidateId = null)
        {
            int resolvedId;

            if (candidateId.HasValue && (User.IsInRole("Admin") || User.IsInRole("Recruiter")))
            {
                resolvedId = candidateId.Value;
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var id = await _candidateService.GetCandidateIdByUserIdAsync(user!.Id);
                if (id == null)
                {
                    TempData["ErrorMessage"] = "Candidate profile not found.";
                    return RedirectToAction("Dashboard", "Candidate");
                }
                resolvedId = id.Value;
            }

            var viewModel = await _openAIService
                .GenerateCandidateRecommendationsAsync(resolvedId);
            return View(viewModel);
        }

        // ─── AI Chatbot Page ───────────────────────────────────────────────
        [HttpGet]
        public IActionResult Chatbot(string? context = null)
        {
            var viewModel = new ChatbotViewModel { Context = context };
            return View(viewModel);
        }

        // ─── Chatbot Message (AJAX POST) ───────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChatMessage(
            [FromBody] ChatbotRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { error = "Message cannot be empty." });

            var history = request.History ?? new List<ChatMessage>();
            var response = await _openAIService.GetChatbotResponseAsync(
                request.Message, history, request.Context);

            return Ok(new
            {
                response,
                timestamp = DateTime.UtcNow.ToString("HH:mm")
            });
        }

        // ─── Regenerate Interview Questions (AJAX) ─────────────────────────
        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegenerateQuestions(
            int jobId, int? applicationId, int count = 10)
        {
            var viewModel = await _openAIService.GenerateInterviewQuestionsAsync(
                jobId, applicationId, count);
            return PartialView("_InterviewQuestionsPartial", viewModel.Questions);
        }

        // ─── API: Quick AI Score ───────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> PredictSuccess(int applicationId)
        {
            var score = await _openAIService.PredictApplicationSuccessAsync(applicationId);
            return Ok(new { successScore = score });
        }
    }

    public class ChatbotRequestModel
    {
        public string Message { get; set; } = string.Empty;
        public List<ChatMessage>? History { get; set; }
        public string? Context { get; set; }
    }
}