using Paddock.BuildingBlocks.Pricing;

namespace Paddock.UnitTests.BuildingBlocks.Pricing;

public sealed class VatBreakdownTests
{
    // Montants TTC en centimes et taux variés, dont des cas où la division ne tombe pas juste.
    public static readonly TheoryData<long, decimal> PricesAndRates = new()
    {
        { 0, 20m },
        { 1, 20m },
        { 3, 20m },
        { 999, 5.5m },
        { 12_345, 10m },
        { 100_001, 2.1m },
        { 4_999_999, 20m },
    };

    [Fact]
    public void Excluding_vat_is_the_price_divided_by_one_plus_the_rate()
    {
        var rate = VatRate.Create(20m).Value;
        var includingVat = Money.Create(12_000).Value; // 120,00 €

        var breakdown = rate.Split(includingVat);

        Assert.Equal(10_000, breakdown.ExcludingVat.Cents); // 100,00 €
        Assert.Equal(2_000, breakdown.Vat.Cents);           // 20,00 €
        Assert.Equal(12_000, breakdown.IncludingVat.Cents);
    }

    [Fact]
    public void A_half_cent_is_rounded_up()
    {
        var rate = VatRate.Create(20m).Value;
        var includingVat = Money.Create(3).Value; // 0,03 € → HT exact 0,025 €

        var breakdown = rate.Split(includingVat);

        // Demi-centime vers le haut (règle de facturation) ; l'arrondi bancaire par défaut de .NET donnerait 2 + 1.
        Assert.Equal(3, breakdown.ExcludingVat.Cents);
        Assert.Equal(0, breakdown.Vat.Cents);
    }

    [Fact]
    public void A_reduced_rate_of_five_and_a_half_percent()
    {
        var rate = VatRate.Create(5.5m).Value;
        var includingVat = Money.Create(1_055).Value; // 10,55 €

        var breakdown = rate.Split(includingVat);

        Assert.Equal(1_000, breakdown.ExcludingVat.Cents); // 10,00 €
        Assert.Equal(55, breakdown.Vat.Cents);             // 0,55 €
    }

    [Fact]
    public void A_zero_rate_has_no_vat()
    {
        var rate = VatRate.Create(0m).Value;
        var includingVat = Money.Create(1_000).Value; // 10,00 €

        var breakdown = rate.Split(includingVat);

        Assert.Equal(1_000, breakdown.ExcludingVat.Cents);
        Assert.Equal(0, breakdown.Vat.Cents);
    }

    [Theory]
    [MemberData(nameof(PricesAndRates))]
    public void The_parts_always_add_up_to_the_price_including_vat(long includingVatCents, decimal percent)
    {
        var rate = VatRate.Create(percent).Value;
        var includingVat = Money.Create(includingVatCents).Value;

        var breakdown = rate.Split(includingVat);

        Assert.Equal(includingVat, breakdown.ExcludingVat + breakdown.Vat);
    }
}
