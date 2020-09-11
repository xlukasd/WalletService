using CSharpFunctionalExtensions;

namespace Domain.Model.ValueObjects
{
    public class Money : ValueObject<Money>
    {
        public static Money Zero => new Money(0m);

        private Money(decimal amount)
        {
            Amount = amount;
        }

        public decimal Amount { get; }

        public static Result<Money> Create(decimal amount)
        {
            if (amount < 0)
            {
                return Result.Failure<Money>("Negative amount.");
            }

            if (amount % 0.01m != 0)
            {
                return Result.Failure<Money>("Too precise amount specification.");
            }

            return Result.Success(new Money(amount));
        }

        protected override bool EqualsCore(Money other)
        {
            return Amount == other.Amount;
        }

        protected override int GetHashCodeCore()
        {
            return Amount.GetHashCode();
        }

        public static Money operator +(Money addend1, Money addend2)
        {
            return new Money(addend1.Amount + addend2.Amount);
        }
    }
}
