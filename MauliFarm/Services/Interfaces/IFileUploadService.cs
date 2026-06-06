namespace MauliFarm.Services.Interfaces
{
    /// <summary>
    /// Contract for file upload operations (profile pictures, future document uploads).
    /// </summary>
    public interface IFileUploadService
    {
        /// <summary>
        /// Saves an uploaded profile picture to wwwroot/uploads/profiles/.
        /// Returns the relative URL path on success.
        /// </summary>
        Task<ServiceResult<string>> SaveProfilePictureAsync(
            IFormFile file,
            string userId,
            IWebHostEnvironment env);

        /// <summary>
        /// Deletes a previously saved profile picture from disk.
        /// </summary>
        Task<bool> DeleteFileAsync(string relativePath, IWebHostEnvironment env);

        /// <summary>
        /// Validates image file: extension, MIME type, and size limit.
        /// </summary>
        ServiceResult ValidateImageFile(IFormFile file, int maxSizeKb = 2048);
    }
}
