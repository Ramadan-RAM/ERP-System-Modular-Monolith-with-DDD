// ============================================
// File: SharedKernel/Exceptions/NotFoundException.cs
// Namespace: ERPSys.SharedKernel.Exceptions
// Purpose: Exception for not found entities
// ============================================

using System;

namespace ERPSys.SharedKernel.Exceptions
{
    /// <summary>
    /// EN: Exception thrown when an entity is not found.
    /// </summary>
    public class NotFoundException : DomainException
    {
        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.") { }
    }
}
