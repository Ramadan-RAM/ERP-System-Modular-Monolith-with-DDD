// Finance.Domain/Common/ValueObjects/DocumentNumber.cs
using ERPSys.SharedKernel.Domain;

namespace Finance.Domain.ValueObjects
{
    // Document/Record number with the ability to add a prefix according to the year/branch
    public sealed class DocumentNumber : ValueObject
    {
        private DocumentNumber() { } // EF
        public string Value { get; }

  
        public DocumentNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Document number required.");
            Value = value.Trim().ToUpperInvariant();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
