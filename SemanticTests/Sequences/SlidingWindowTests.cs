using System;
using Semantic.Sequences;

namespace SemanticTests.Sequences;

public class SlidingWindowTests
{
    [Fact]
    public void TestSlidingWindowInvalidSizeThrows()
    {
        Assert.Throws<ArgumentException>(() => SlidingWindowBuilder.Create<int>(0));
        Assert.Throws<ArgumentException>(() => SlidingWindowBuilder.Create<int>(-1));
    }

    [Fact]
    public void TestSlidingWindowSizeProperty()
    {
        var window = SlidingWindowBuilder.Create<int>(5);
        Assert.Equal(5, window.Size);
    }

    [Fact]
    public void TestSlidingWindowSizeOne()
    {
        var window = SlidingWindowBuilder.Create<int>(1);

        Assert.Empty(window.GetItems());

        window.Add(1);
        Assert.Equal([1], window.GetItems());

        // Each new item replaces the previous one
        window.Add(2);
        Assert.Equal([2], window.GetItems());

        window.Add(3);
        Assert.Equal([3], window.GetItems());
    }

    [Fact]
    public void TestSlidingWindow()
    {
        var window = SlidingWindowBuilder.Create<int>(3);

        Assert.Empty(window.GetItems());

        window.Add(1);
        Assert.Equal([1], window.GetItems());

        window.Add(2);
        Assert.Equal([1, 2], window.GetItems());

        window.Add(3);
        Assert.Equal([1, 2, 3], window.GetItems());

        // Adding a new item should remove the oldest item (1)
        window.Add(4);
        Assert.Equal([2, 3, 4], window.GetItems());

        // Adding another item should remove the next oldest item (2)
        window.Add(5);
        Assert.Equal([3, 4, 5], window.GetItems());
    }

    [Fact]
    public void TestSlidingWindowClear()
    {
        var window = SlidingWindowBuilder.Create<int>(3);
        window.Add(1);
        window.Add(2);
        window.Add(3);

        Assert.Equal([1, 2, 3], window.GetItems());

        window.Clear();
        Assert.Empty(window.GetItems());

        // After clearing, adding new items should work as expected
        window.Add(4);
        Assert.Equal([4], window.GetItems());

        window.Add(5);
        Assert.Equal([4, 5], window.GetItems());
    }

    [Fact]
    public void TestSlidingWindowClearAfterSliding()
    {
        var window = SlidingWindowBuilder.Create<int>(3);
        window.Add(1);
        window.Add(2);
        window.Add(3);
        window.Add(4); // window is now sliding: [2, 3, 4]

        window.Clear();
        Assert.Empty(window.GetItems());

        // State is fully reset — should behave like a new window
        window.Add(10);
        Assert.Equal([10], window.GetItems());

        window.Add(20);
        Assert.Equal([10, 20], window.GetItems());

        window.Add(30);
        Assert.Equal([10, 20, 30], window.GetItems());

        window.Add(40);
        Assert.Equal([20, 30, 40], window.GetItems());
    }
}
