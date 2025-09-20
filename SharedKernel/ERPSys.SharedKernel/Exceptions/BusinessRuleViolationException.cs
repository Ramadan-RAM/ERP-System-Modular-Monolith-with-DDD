// ============================================
// File: SharedKernel/Exceptions/BusinessRuleViolationException.cs
// Namespace: ERPSys.SharedKernel.Exceptions
// Purpose: Exception for business rule violations
// ============================================

using System;

namespace ERPSys.SharedKernel.Exceptions
{
    /// <summary>
    /// EN: Exception thrown when a business rule is violated.
    /// </summary>
    public class BusinessRuleViolationException : DomainException
    {
        public BusinessRuleViolationException(string message) : base(message) { }
    }
}
