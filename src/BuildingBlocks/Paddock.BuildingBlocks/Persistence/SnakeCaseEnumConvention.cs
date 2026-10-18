using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Paddock.BuildingBlocks.Persistence;

// Applique SnakeCaseEnumConverter à toute propriété enum (nullable comprise) du modèle, sans configuration par entité.
// Une conversion configurée explicitement sur une propriété reste prioritaire.
// Enregistrement dans un DbContext : ConfigureConventions(b) => b.Conventions.Add(_ => new SnakeCaseEnumConvention());
public sealed class SnakeCaseEnumConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        var properties = modelBuilder.Metadata.GetEntityTypes().SelectMany(entityType => entityType.GetDeclaredProperties());

        foreach (var property in properties)
        {
            var enumType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
            if (!enumType.IsEnum)
            {
                continue;
            }

            // Le type de l'enum n'est connu qu'à l'exécution : on fabrique SnakeCaseEnumConverter<TEnum> par réflexion.
            var converterType = typeof(SnakeCaseEnumConverter<>).MakeGenericType(enumType);
            var converter = (ValueConverter)Activator.CreateInstance(converterType)!;

            property.Builder.HasConversion(converter);
        }
    }
}
