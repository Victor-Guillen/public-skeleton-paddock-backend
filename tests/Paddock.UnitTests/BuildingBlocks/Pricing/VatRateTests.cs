using Paddock.BuildingBlocks.Pricing;

namespace Paddock.UnitTests.BuildingBlocks.Pricing;

public sealed class VatRateTests
{
    // [InlineData] n'accepte pas les decimal (constantes d'attribut) : les données passent par des TheoryData.
    public static readonly TheoryData<decimal> ValidRates = [20m, 10m, 5.5m, 2.1m, 0m, 100m];

    public static readonly TheoryData<decimal> OutOfRangeRates = [-1m, -0.01m, 100.01m];

    [Theory]
    [MemberData(nameof(ValidRates))]
    public void The_french_rates_and_the_bounds_are_valid(decimal percent)
    {
        var result = VatRate.Create(percent);

        Assert.True(result.IsSuccess);
        Assert.Equal(percent, result.Value.Percent);
    }

    [Theory]
    [MemberData(nameof(OutOfRangeRates))]
    public void A_vat_rate_must_be_between_zero_and_one_hundred(decimal percent)
    {
        var result = VatRate.Create(percent);

        Assert.True(result.IsFailure);
        Assert.Equal("common.vat_rate_out_of_range", result.Error.Code);
    }

    [Fact]
    public void A_vat_rate_cannot_have_more_than_two_decimals()
    {
        var result = VatRate.Create(5.555m);

        Assert.True(result.IsFailure);
        Assert.Equal("common.vat_rate_too_precise", result.Error.Code);
    }
}
