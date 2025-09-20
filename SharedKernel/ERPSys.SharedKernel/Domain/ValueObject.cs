// ============================================
// File: SharedKernel/Domain/ValueObject.cs
// Namespace: ERPSys.SharedKernel.Domain
// Purpose: DDD Value Object base with equality semantics
// ============================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPSys.SharedKernel.Domain
{
    /// <summary>
    /// EN: Base class for Value Objects (immutable by design).
    /// Provides equality comparison by components (structural equality).
    /// Use for types like Money, AccountCode, Address that are identified by their values.
    /// 
    /// AR: قاعدة للـ Value Objects (مصممة لتكون غير قابلة للتغيير عادة).
    /// تُوفّر منطق المقارنة بالمحتوى (Equality) عن طريق مكونات القيمة.
    /// استخدمها لأنواع زي Money, AccountCode, Address حيث الهوية بالمحتوى مش بالـ Id.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Implement this to return the sequence of components that define equality.
        /// Example: for Money => return new object[] { Amount, Currency };
        /// </summary>
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (ValueObject)obj;

            using var thisValues = GetEqualityComponents().GetEnumerator();
            using var otherValues = other.GetEqualityComponents().GetEnumerator();

            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                var a = thisValues.Current;
                var b = otherValues.Current;

                if (a is null ^ b is null) return false;
                if (a != null && !a.Equals(b)) return false;
            }

            // Ensure both enumerations reached the end
            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        public override int GetHashCode()
        {
            // Combine hashcodes of all components in a stable manner
            unchecked
            {
                const int seed = 17;
                const int mult = 23;

                return GetEqualityComponents()
                    .Where(c => c != null)
                    .Aggregate(seed, (current, obj) => (current * mult) + obj!.GetHashCode());
            }
        }

        public static bool operator ==(ValueObject? left, ValueObject? right) =>
            Equals(left, right);

        public static bool operator !=(ValueObject? left, ValueObject? right) =>
            !Equals(left, right);
    }
}
