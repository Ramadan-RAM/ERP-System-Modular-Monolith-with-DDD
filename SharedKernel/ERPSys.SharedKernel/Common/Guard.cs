// ============================================
// File: SharedKernel/Common/Guard.cs
// Namespace: ERPSys.SharedKernel.Common
// Purpose: Defensive checks helper
// ============================================

using System;

namespace ERPSys.SharedKernel.Common
{
    /// <summary>
    /// EN: Guard clauses to validate arguments and throw exceptions.
    /// 
    /// </summary>
    public static class Guard
    {
        public static void AgainstNull(object? value, string name)
        {
            if (value is null)
                throw new ArgumentNullException(name);
        }

        public static void AgainstNullOrEmpty(string? value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{name} cannot be null or empty.");
        }

        public static void AgainstOutOfRange<T>(T value, T min, T max, string name)
            where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException(name, $"{name} must be between {min} and {max}.");
        }
    }
}
