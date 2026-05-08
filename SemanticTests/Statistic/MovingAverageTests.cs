using System;
using Semantic.Statistic;

namespace SemanticTests.Statistic;

public class MovingAverageTests
{
    [Fact]
    public void TestMovingAverageInvalidSizeThrows()
    {
        Assert.Throws<ArgumentException>(() => MovingAverageBuilder.Create(0));
        Assert.Throws<ArgumentException>(() => MovingAverageBuilder.Create(-1));
    }

    [Fact]
    public void TestMovingAverageInitialState()
    {
        var movingAverage = MovingAverageBuilder.Create(3);
        Assert.True(double.IsNaN(movingAverage.CurrentAverage));
    }

    [Fact]
    public void TestMovingAverageAverageUpdatedEvent()
    {
        var movingAverage = MovingAverageBuilder.Create(3);
        var eventValues = new List<double>();
        movingAverage.AverageUpdated += (_, avg) => eventValues.Add(avg);

        movingAverage.Add(1);
        movingAverage.Add(2);
        movingAverage.Add(3);

        Assert.Equal([1, 1.5, 2], eventValues);
    }

    [Fact]
    public void TestMovingAverageSliding()
    {
        var movingAverage = MovingAverageBuilder.Create(3);
        movingAverage.Add(1);
        movingAverage.Add(2);
        movingAverage.Add(3);
        Assert.Equal(2, movingAverage.CurrentAverage);

        // Window slides: drops 1, adds 4 → [2, 3, 4]
        movingAverage.Add(4);
        Assert.Equal(3, movingAverage.CurrentAverage);

        // Window slides: drops 2, adds 5 → [3, 4, 5]
        movingAverage.Add(5);
        Assert.Equal(4, movingAverage.CurrentAverage);
    }

    [Fact]
    public void TestMovingAverageWithNaNValues()
    {
        var movingAverage = MovingAverageBuilder.Create(3);
        movingAverage.Add(double.NaN);
        movingAverage.Add(2);
        Assert.Equal(2, movingAverage.CurrentAverage); // NaN is skipped

        // All-NaN window should yield NaN
        var allNaN = MovingAverageBuilder.Create(2);
        allNaN.Add(double.NaN);
        allNaN.Add(double.NaN);
        Assert.True(double.IsNaN(allNaN.CurrentAverage));
    }

    [Fact]
    public void TestMovingAverage()
    {
        var movingAverage = MovingAverageBuilder.Create(3);
        movingAverage.Add(1);
        movingAverage.Add(2);
        movingAverage.Add(3);
        Assert.Equal(2, movingAverage.CurrentAverage);
    }

    [Fact]
    public void TestMovingAverageCustomStep()
    {
        var movingAverage = MovingAverageBuilder.CreateCustomStep(3, 2);
        movingAverage.Add(1);
        Assert.True(double.IsNaN(movingAverage.CurrentAverage)); // Not enough data points yet
        movingAverage.Add(2);
        Assert.Equal(1.5, movingAverage.CurrentAverage); // Average of [1, 2]
        movingAverage.Add(3);
        Assert.Equal(2.5, movingAverage.CurrentAverage); // Average of [2, 3]
    }

    [Fact]
    public void TestMovingAverageCustomStepInvalidSampleSizeThrows()
    {
        Assert.Throws<ArgumentException>(() => MovingAverageBuilder.CreateCustomStep(3, 0));
        Assert.Throws<ArgumentException>(() => MovingAverageBuilder.CreateCustomStep(3, -1));
    }

    [Fact]
    public void TestMovingAverageCustomStepAverageUpdatedEvent()
    {
        var movingAverage = MovingAverageBuilder.CreateCustomStep(3, 2);
        var eventValues = new List<double>();
        movingAverage.AverageUpdated += (_, avg) => eventValues.Add(avg);

        movingAverage.Add(1); // not enough — no event
        movingAverage.Add(2); // event: avg of [1, 2] = 1.5
        movingAverage.Add(3); // event: avg of [2, 3] = 2.5

        Assert.Equal([1.5, 2.5], eventValues);
    }

    [Fact]
    public void TestMovingAverageCustomStepSliding()
    {
        // Window size 3, sample size 2: always averages the 2 most recent items
        var movingAverage = MovingAverageBuilder.CreateCustomStep(3, 2);
        movingAverage.Add(1);
        movingAverage.Add(2);
        movingAverage.Add(3); // window: [1, 2, 3] → avg of [2, 3] = 2.5
        Assert.Equal(2.5, movingAverage.CurrentAverage);

        movingAverage.Add(10); // window slides: [2, 3, 10] → avg of [3, 10] = 6.5
        Assert.Equal(6.5, movingAverage.CurrentAverage);
    }
}
