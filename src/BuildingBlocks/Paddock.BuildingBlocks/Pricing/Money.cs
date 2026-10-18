using Paddock.BuildingBlocks.Results;

namespace Paddock.BuildingBlocks.Pricing;

public readonly record struct Money
{
    public long Cents { get; }
    public const string Currency = "EUR";
    public static readonly Error NegativeAmount =
        Error.Validation("common.negative_amount", "An amount cannot be negative.");
    public static readonly Error PercentNotInRange =
        Error.Validation("common.percent_not_in_range", "A percent must be between 0 and 100.");

    private Money(long cents)
    {
        Cents = cents;
    }

    public static Money Zero => new(0);

    public static Result<Money> Create(long cents)
    {
        if (cents < 0)
        {
            return NegativeAmount;
        }
        return new Money(cents);
    }

    public static Money operator +(Money left, Money right) =>
        new(checked(left.Cents + right.Cents));

    public static Result<Money> Percentage(Money amount, int percent)
    {
        if (percent < 0 || percent > 100)
        {
            return PercentNotInRange;
        }
        return new Money(checked(amount.Cents * percent) / 100);
    }

}
