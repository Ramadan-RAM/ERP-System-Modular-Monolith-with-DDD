// Finance.Domain/Entities/Currency.cs
using ERPSys.SharedKernel.Domain;

namespace Finance.Domain.Entities
{
    //Define the system currency and link it to exchange rates
    public class Currency : BaseEntity<int>, IAggregateRoot
    {
        public string Code { get; private set; } = default!; // "USD"
        public string Name { get; private set; } = default!;
        public bool IsBase { get; private set; }

        private Currency() { }
        public Currency(string code, string name, bool isBase = false)
        {
            Code = code.ToUpperInvariant();
            Name = name.Trim();
            IsBase = isBase;
        }

        public void SetBase(bool isBase) => IsBase = isBase;
    }
}
