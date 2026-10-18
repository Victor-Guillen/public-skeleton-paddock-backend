using System.Buffers.Text;
using System.Text;
using Paddock.BuildingBlocks.Pagination;

namespace Paddock.UnitTests.BuildingBlocks.Pagination;

public sealed class CursorTests
{
    [Fact]
    public void A_cursor_round_trips_its_position()
    {
        var position = new TestPosition(new DateOnly(2026, 5, 14), Guid.NewGuid());

        var decoded = Cursor.Decode<TestPosition>(Cursor.Encode(position));

        Assert.True(decoded.IsSuccess);
        Assert.Equal(position, decoded.Value);
    }

    [Fact]
    public void A_cursor_can_be_used_in_a_url_as_is()
    {
        var cursor = Cursor.Encode(new TestPosition(new DateOnly(2026, 5, 14), Guid.NewGuid()));

        Assert.DoesNotContain('+', cursor);
        Assert.DoesNotContain('/', cursor);
        Assert.DoesNotContain('=', cursor);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not a cursor!")]
    public void A_cursor_that_is_not_base64_is_invalid(string cursor)
    {
        var decoded = Cursor.Decode<TestPosition>(cursor);

        Assert.True(decoded.IsFailure);
        Assert.Equal("common.invalid_cursor", decoded.Error.Code);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("""{"date":"2026-05-14"}""")]
    public void A_cursor_with_an_unexpected_content_is_invalid(string json)
    {
        var cursor = Base64Url.EncodeToString(Encoding.UTF8.GetBytes(json));

        var decoded = Cursor.Decode<TestPosition>(cursor);

        Assert.True(decoded.IsFailure);
        Assert.Equal("common.invalid_cursor", decoded.Error.Code);
    }

    private sealed record TestPosition(DateOnly Date, Guid Id);
}
