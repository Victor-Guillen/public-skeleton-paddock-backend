namespace Paddock.BuildingBlocks.Pricing;

// Décomposition d'un prix TTC. Construite par VatRate.Split, qui garantit ExcludingVat + Vat = IncludingVat.
public sealed record VatBreakdown(Money ExcludingVat, Money Vat, Money IncludingVat);
