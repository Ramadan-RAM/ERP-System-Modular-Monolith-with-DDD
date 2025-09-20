// Finance.Domain/Common/ValueObjects/AccountCode.cs
using ERPSys.SharedKernel.Domain;

namespace Finance.Domain.ValueObjects
{
    // Accounting account code (includes format and order)
    public sealed class AccountCode : ValueObject
    {
        public string Value { get; }

        private AccountCode() { } // EF
        public AccountCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Account code required.");
            Value = value.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
