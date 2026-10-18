using Paddock.BuildingBlocks.Pagination;

namespace Paddock.UnitTests.BuildingBlocks.Pagination;

public sealed class CursorPageTests
{
    private const int Limit = 3;

    [Fact]
    public void An_extra_fetched_item_means_there_is_a_next_page()
    {
        var page = CursorPage.From(Items(Limit + 1), Limit, item => item);

        Assert.True(page.HasMore);
        Assert.Equal([1, 2, 3], page.Items);
    }

    [Fact]
    public void The_next_cursor_points_to_the_last_returned_item()
    {
        var page = CursorPage.From(Items(Limit + 1), Limit, item => item);

        Assert.NotNull(page.NextCursor);
        Assert.Equal(3, Cursor.Decode<int>(page.NextCursor).Value);
    }

    [Fact]
    public void Without_an_extra_item_the_page_is_the_last_one()
    {
        var page = CursorPage.From(Items(Limit), Limit, item => item);

        Assert.False(page.HasMore);
        Assert.Null(page.NextCursor);
        Assert.Equal([1, 2, 3], page.Items);
    }

    [Fact]
    public void An_empty_result_is_an_empty_last_page()
    {
        var page = CursorPage.From(Items(0), Limit, item => item);

        Assert.Empty(page.Items);
        Assert.False(page.HasMore);
        Assert.Null(page.NextCursor);
    }

    private static List<int> Items(int count) => [.. Enumerable.Range(1, count)];
}
