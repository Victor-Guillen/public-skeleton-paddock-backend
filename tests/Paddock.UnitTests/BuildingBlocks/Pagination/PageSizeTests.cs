using Paddock.BuildingBlocks.Pagination;

namespace Paddock.UnitTests.BuildingBlocks.Pagination;

public sealed class PageSizeTests
{
    [Fact]
    public void A_missing_limit_uses_the_default_size()
    {
        Assert.Equal(20, PageSize.Normalize(null));
    }

    [Theory]
    [InlineData(25, 25)]
    [InlineData(500, 50)]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    public void A_limit_is_brought_back_within_bounds(int limit, int expected)
    {
        Assert.Equal(expected, PageSize.Normalize(limit));
    }
}
