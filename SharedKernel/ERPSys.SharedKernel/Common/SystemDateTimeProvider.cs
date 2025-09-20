// ============================================
// File: SharedKernel/Common/SystemDateTimeProvider.cs
// Namespace: ERPSys.SharedKernel.Common
// Purpose: Default implementation of IDateTimeProvider
// ============================================

using System;

namespace ERPSys.SharedKernel.Common
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Now => DateTime.Now;
    }
}
