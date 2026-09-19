using System;

namespace Semantic.Statistic;

public interface IMovingAverage
{
    double CurrentAverage { get; }
    void Add(double value);

    event EventHandler<double>? AverageUpdated;
}
