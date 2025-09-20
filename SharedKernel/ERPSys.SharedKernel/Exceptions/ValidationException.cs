// ============================================
// File: SharedKernel/Exceptions/ValidationException.cs
// Namespace: ERPSys.SharedKernel.Exceptions
// Purpose: Exception for validation errors
// ============================================

using System;
using System.Collections.Generic;

namespace ERPSys.SharedKernel.Exceptions
{
    /// <summary>
    /// EN: Exception for validation failures with multiple errors.
    /// </summary>
    public class ValidationException : DomainException
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("Validation failed")
        {
            Errors = errors;
        }
    }
}
