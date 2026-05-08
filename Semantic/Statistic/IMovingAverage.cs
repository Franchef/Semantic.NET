using System;

namespace Semantic.Statistic;

public interface IMovingAverage
{
    public double CurrentAverage { get; }
    public void Add(double value);

    event EventHandler<double>? AverageUpdated;
}
