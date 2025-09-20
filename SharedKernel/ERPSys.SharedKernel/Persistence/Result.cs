// ============================================
// File: SharedKernel/Results/Result.cs
// Namespace: ERPSys.SharedKernel.Results
// Purpose: Unified Result (Success / Failure)
// ============================================

namespace ERPSys.SharedKernel.Persistence
{
    /// <summary>
    /// EN: Represents the outcome of an operation (success/failure + message + optional data).
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Message { get; }
        public string? Error => !IsSuccess ? Message : null;

        protected Result(bool isSuccess, string? message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static Result Success(string message = "") => new Result(true, message);
        public static Result Failure(string error) => new Result(false, error);
    }

    /// <summary>
    /// EN: Generic Result with data.
    /// </summary>
    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool isSuccess, T? data, string? message)
            : base(isSuccess, message)
        {
            Data = data;
        }

        public static Result<T> Success(T data, string message = "") =>
            new Result<T>(true, data, message);

        public static new Result<T> Failure(string error) =>
            new Result<T>(false, default, error);
    }
}
