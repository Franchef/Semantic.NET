namespace Semantic.Statistic;

internal sealed class MovingAverageCustomStep : MovingAverage
{
    private readonly int _sampleSize;

    public MovingAverageCustomStep(int size, int sampleSize) : base(size)
    {
        if(sampleSize < 1)
        {
            throw new ArgumentException("Step must be greater than 0.", nameof(sampleSize));
        }
        _sampleSize = sampleSize;
    }

    protected override void UpdateAverage()
    {
        var items = _window.GetItems();
        if(items.Length < _sampleSize)
        {
            CurrentAverage = double.NaN;
            return;
        }
        double sum = 0;
        for(int i = items.Length - _sampleSize; i < items.Length; i++)
        {
            sum += items[i];
        }
        CurrentAverage = sum / _sampleSize;
        OnAverageUpdated();
    }
}