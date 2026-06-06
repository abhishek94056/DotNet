namespace MauliFarm.Services
{
    /// <summary>
    /// Standardised service result — carries success/failure state,
    /// user-facing message, and optional errors list.
    /// Used by all service methods so controllers stay thin.
    /// </summary>
    public class ServiceResult
    {
        public bool            IsSuccess { get; protected set; }
        public string          Message   { get; protected set; } = string.Empty;
        public List<string>    Errors    { get; protected set; } = new();

        // ── Factory methods ───────────────────────────────────────────────

        public static ServiceResult Success(string message = "Operation completed successfully.")
            => new() { IsSuccess = true, Message = message };

        public static ServiceResult Failure(string message, IEnumerable<string>? errors = null)
            => new()
            {
                IsSuccess = false,
                Message   = message,
                Errors    = errors?.ToList() ?? new List<string>()
            };

        public static ServiceResult Failure(IEnumerable<string> errors)
            => new()
            {
                IsSuccess = false,
                Message   = "One or more validation errors occurred.",
                Errors    = errors.ToList()
            };
    }

    /// <summary>
    /// Generic variant that also returns a typed payload on success.
    /// </summary>
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; private set; }

        public static ServiceResult<T> Success(T data, string message = "Operation completed successfully.")
            => new() { IsSuccess = true, Message = message, Data = data };

        public new static ServiceResult<T> Failure(string message, IEnumerable<string>? errors = null)
            => new()
            {
                IsSuccess = false,
                Message   = message,
                Errors    = errors?.ToList() ?? new List<string>()
            };
    }
}
