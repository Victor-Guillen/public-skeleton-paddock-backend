using Paddock.BuildingBlocks.Results;

namespace Paddock.BuildingBlocks.Pricing;

// Taux de TVA en pourcentage (20 % → 20m, 5,5 % → 5.5m), figé dans chaque réservation.
// default(VatRate) vaut 0 % : c'est un taux légitime (franchise en base de TVA), donc pas un état invalide.
// Le risque est un taux oublié qui passerait silencieusement à 0 % : il est couvert par la construction des
// entités (le taux est un paramètre obligatoire, issu des réglages de l'organisateur) et par une colonne NOT NULL.
public readonly record struct VatRate
{
    public static readonly Error OutOfRange =
        Error.Validation("common.vat_rate_out_of_range", "A VAT rate must be between 0 and 100.");

    public static readonly Error TooPrecise =
        Error.Validation("common.vat_rate_too_precise", "A VAT rate cannot have more than two decimals.");

    private VatRate(decimal percent)
    {
        Percent = percent;
    }

    public decimal Percent { get; }

    public static Result<VatRate> Create(decimal percent)
    {
        if (percent < 0 || percent > 100)
        {
            return OutOfRange;
        }

        // Les taux français ont au plus une décimale ; au-delà de deux, la valeur serait tronquée en base
        // (numeric(5,2)) ou trahirait une erreur de saisie.
        if (decimal.Round(percent, 2) != percent)
        {
            return TooPrecise;
        }

        return new VatRate(percent);
    }

    // HT = TTC / (1 + taux), arrondi au centime le plus proche (demi-centime vers le haut) ;
    // la TVA est la différence, pour que HT + TVA = TTC au centime près.
    public VatBreakdown Split(Money includingVat)
    {
        var exactExcludingVat = includingVat.Cents / (1 + (Percent / 100));
        var excludingVatCents = (long)decimal.Round(exactExcludingVat, MidpointRounding.AwayFromZero);

        // Toujours positif : HT ≤ TTC, et TTC ≥ 0. Create ne peut donc pas échouer ici.
        var excludingVat = Money.Create(excludingVatCents).Value;
        var vat = Money.Create(includingVat.Cents - excludingVatCents).Value;

        return new VatBreakdown(excludingVat, vat, includingVat);
    }

}
