using MauliFarm.Services.Interfaces;

namespace MauliFarm.Services
{
    /// <summary>
    /// Handles file upload operations for profile pictures.
    /// Saves to wwwroot/uploads/profiles/{userId}_{guid}.{ext}
    /// </summary>
    public class FileUploadService : IFileUploadService
    {
        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

        private static readonly HashSet<string> AllowedMimeTypes =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg", "image/jpg", "image/png", "image/webp"
            };

        private const string ProfileUploadFolder = "uploads/profiles";

        private readonly ILogger<FileUploadService> _logger;

        public FileUploadService(ILogger<FileUploadService> logger)
            => _logger = logger;

        // ─────────────────────────────────────────────────────────────────
        //  SAVE
        // ─────────────────────────────────────────────────────────────────

        public async Task<ServiceResult<string>> SaveProfilePictureAsync(
            IFormFile file,
            string userId,
            IWebHostEnvironment env)
        {
            var validation = ValidateImageFile(file);
            if (!validation.IsSuccess)
                return ServiceResult<string>.Failure(validation.Message, validation.Errors);

            try
            {
                var uploadDir = Path.Combine(env.WebRootPath, ProfileUploadFolder);
                Directory.CreateDirectory(uploadDir);   // ensure folder exists

                var extension    = Path.GetExtension(file.FileName).ToLowerInvariant();
                var fileName     = $"{userId}_{Guid.NewGuid():N}{extension}";
                var fullPath     = Path.Combine(uploadDir, fileName);
                var relativePath = $"/{ProfileUploadFolder}/{fileName}";

                await using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                await file.CopyToAsync(stream);

                _logger.LogInformation("Profile picture saved: {Path}", relativePath);
                return ServiceResult<string>.Success(relativePath, "Profile picture uploaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving profile picture for user {UserId}", userId);
                return ServiceResult<string>.Failure("Failed to save the profile picture. Please try again.");
            }
        }

        // ─────────────────────────────────────────────────────────────────
        //  DELETE
        // ─────────────────────────────────────────────────────────────────

        public async Task<bool> DeleteFileAsync(string relativePath, IWebHostEnvironment env)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativePath)) return false;

                // Strip leading slash
                var cleanPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var fullPath  = Path.Combine(env.WebRootPath, cleanPath);

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    _logger.LogInformation("Profile picture deleted: {Path}", relativePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete file: {Path}", relativePath);
                return false;
            }
        }

        // ─────────────────────────────────────────────────────────────────
        //  VALIDATE
        // ─────────────────────────────────────────────────────────────────

        public ServiceResult ValidateImageFile(IFormFile file, int maxSizeKb = 2048)
        {
            if (file == null || file.Length == 0)
                return ServiceResult.Failure("Please select a file to upload.");

            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
                return ServiceResult.Failure(
                    $"Invalid file type. Allowed: {string.Join(", ", AllowedExtensions)}");

            if (!AllowedMimeTypes.Contains(file.ContentType))
                return ServiceResult.Failure("Invalid file content type.");

            var maxBytes = maxSizeKb * 1024L;
            if (file.Length > maxBytes)
                return ServiceResult.Failure(
                    $"File size exceeds the limit of {maxSizeKb / 1024} MB.");

            return ServiceResult.Success();
        }
    }
}
