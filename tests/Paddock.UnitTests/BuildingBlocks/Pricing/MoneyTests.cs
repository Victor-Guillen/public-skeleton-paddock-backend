using Paddock.BuildingBlocks.Pricing;

namespace Paddock.UnitTests.BuildingBlocks.Pricing;

public sealed class MoneyTests
{
    [Fact]
    public void A_negative_amount_is_rejected()
    {
        var result = Money.Create(-1);

        Assert.True(result.IsFailure);
        Assert.Equal("common.negative_amount", result.Error.Code);
    }

    [Fact]
    public void Zero_is_a_valid_amount()
    {
        var zeroValueResult = Money.Create(0);
        var zeroPropsResult = Money.Zero;

        Assert.True(zeroValueResult.IsSuccess);
        Assert.Equal(0, zeroValueResult.Value.Cents);
        Assert.Equal(0, zeroPropsResult.Cents);
    }

    [Fact]
    public void Adding_two_amounts_sums_their_cents()
    {
        var result = Money.Create(1050).Value + Money.Create(225).Value;

        Assert.Equal(1275, result.Cents);
    }

    [Fact]
    public void Adding_amounts_beyond_the_maximum_throws()
    {
        var maximum = Money.Create(long.MaxValue).Value;
        var oneCent = Money.Create(1).Value;

        Assert.Throws<OverflowException>(() => maximum + oneCent);
    }

    [Fact]
    public void A_percentage_is_rounded_down_to_the_cent()
    {
        var amount = Money.Create(1001).Value; // 10,01 €
        var result = Money.Percentage(amount, 33); // 33 % = 3,3033 €

        Assert.True(result.IsSuccess);
        Assert.Equal(330, result.Value.Cents); // 3,30 € : le reste (0,0033 €) est abandonné
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(100, 1001)]
    public void Zero_percent_is_nothing_and_one_hundred_percent_is_everything(int percent, long expected)
    {
        var amount = Money.Create(1001).Value; // 10,01 €
        var result = Money.Percentage(amount, percent);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value.Cents);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void A_percentage_outside_zero_to_one_hundred_is_rejected(int percent)
    {
        var amount = Money.Create(1001).Value; // 10,01 €
        var result = Money.Percentage(amount, percent); // -1 % or 101 %

        Assert.True(result.IsFailure);
        Assert.Equal("common.percent_not_in_range", result.Error.Code);
    }
}
