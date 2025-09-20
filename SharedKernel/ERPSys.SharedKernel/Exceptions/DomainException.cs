// ============================================
// File: SharedKernel/Exceptions/DomainException.cs
// Namespace: ERPSys.SharedKernel.Exceptions
// Purpose: Base exception for domain errors
// ============================================

using System;

namespace ERPSys.SharedKernel.Exceptions
{
    /// <summary>
    /// EN: Base exception for domain-related errors.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }
}
