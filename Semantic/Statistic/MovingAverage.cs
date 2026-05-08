using Semantic.Sequences;

namespace Semantic.Statistic;

internal class MovingAverage : IMovingAverage
{
    protected readonly ISlidingWindow<double> _window;

    public MovingAverage(int size)
    {
         if(size < 1)
        {
            throw new ArgumentException("Size must be greater than 0.", nameof(size));
        }
        _window = SlidingWindowBuilder.Create<double>(size);
    }
    public double CurrentAverage { get; protected set; } = double.NaN;

    public event EventHandler<double>? AverageUpdated;

    public void Add(double value)
    {
        _window.Add(value);
        UpdateAverage();
    }

    protected void OnAverageUpdated() => AverageUpdated?.Invoke(this, CurrentAverage);

    protected virtual void UpdateAverage()
    {
        CurrentAverage = GetAverage();
        OnAverageUpdated();
    }

    protected double GetAverage()
    {
        var items = _window.GetItems();
        switch(items.Length)
        {
            case 0:
                return double.NaN;
            case 1:
                return items[0];
            default:
                double sum = 0;
                double count = 0;
                for(int i = 0; i < items.Length; i++)
                {
                    if(double.IsNaN(items[i])) continue;
                    sum += items[i];
                    count++;
                }
                return count > 0 ? sum / count : double.NaN;
        }
    }
}