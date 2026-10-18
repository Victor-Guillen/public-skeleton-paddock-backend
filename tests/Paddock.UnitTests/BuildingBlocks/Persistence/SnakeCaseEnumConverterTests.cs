using Paddock.BuildingBlocks.Persistence;

namespace Paddock.UnitTests.BuildingBlocks.Persistence;

public sealed class SnakeCaseEnumConverterTests
{
    private readonly SnakeCaseEnumConverter<TestStatus> _converter = new();

    [Theory]
    [InlineData(TestStatus.Draft, "draft")]
    [InlineData(TestStatus.PendingPayment, "pending_payment")]
    public void An_enum_value_is_stored_as_snake_case_text(TestStatus status, string expected)
    {
        Assert.Equal(expected, _converter.ConvertToProvider(status));
    }

    [Theory]
    [InlineData("draft", TestStatus.Draft)]
    [InlineData("pending_payment", TestStatus.PendingPayment)]
    public void Snake_case_text_is_read_back_as_the_enum_value(string stored, TestStatus expected)
    {
        Assert.Equal(expected, _converter.ConvertFromProvider(stored));
    }

    [Fact]
    public void An_unknown_stored_value_is_rejected()
    {
        Assert.Throws<InvalidOperationException>(() => _converter.ConvertFromProvider("archived"));
    }

    // public : utilisé comme paramètre d'une méthode de test publique ([Theory]).
    public enum TestStatus
    {
        Draft,
        PendingPayment,
    }
}
