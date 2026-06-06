namespace AIResumeScreeningSystem.Helpers
{
    public static class FileHelper
    {
        private static readonly string[] AllowedExtensions = { ".pdf", ".docx", ".doc" };
        private static readonly string[] AllowedMimeTypes =
        {
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/msword"
        };

        public static bool IsValidResumeFile(IFormFile file, int maxSizeMB = 5)
        {
            if (file == null || file.Length == 0) return false;
            if (file.Length > maxSizeMB * 1024 * 1024) return false;

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(ext)) return false;

            return true;
        }

        public static string GetValidationError(IFormFile file, int maxSizeMB = 5)
        {
            if (file == null || file.Length == 0)
                return "Please select a file.";

            if (file.Length > maxSizeMB * 1024 * 1024)
                return $"File size must not exceed {maxSizeMB}MB. Your file is {file.Length / (1024.0 * 1024):F1}MB.";

            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(ext))
                return $"Only PDF, DOCX, and DOC files are allowed. You uploaded: {ext}";

            return string.Empty;
        }

        public static async Task<string> SaveFileAsync(
            IFormFile file,
            string folderPath,
            string? customFileName = null)
        {
            Directory.CreateDirectory(folderPath);

            var ext = Path.GetExtension(file.FileName).ToLower();
            var fileName = customFileName != null
                ? $"{customFileName}{ext}"
                : $"{Guid.NewGuid()}{ext}";

            var fullPath = Path.Combine(folderPath, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public static string SanitizeFileName(string fileName)
        {
            var invalid = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries))
                         .TrimEnd('.');
        }
    }
}