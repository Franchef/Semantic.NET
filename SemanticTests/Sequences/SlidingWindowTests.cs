using System;
using Semantic.Sequences;

namespace SemanticTests.Sequences;

public class SlidingWindowTests
{
    [Fact]
    public void TestSlidingWindow()
    {
        var window = new SlidingWindow<int>(3);

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
        var window = new SlidingWindow<int>(3);
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
}
