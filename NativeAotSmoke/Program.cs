using Semantic.Sequences;
using Semantic.Statistic;

const double Tolerance = 0.0001d;

var pattern = PatternMatchesBuilder.Create(1, 2, 3);
pattern.Next(1);
pattern.Next(2);
pattern.Next(3);
if (!pattern.HasMatch())
{
    throw new InvalidOperationException("PatternMatchesBuilder smoke test failed.");
}

var window = SlidingWindowBuilder.Create<int>(3);
window.Add(1);
window.Add(2);
window.Add(3);
window.Add(4);
var items = window.GetItems();
if (items.Length != 3 || items[0] != 2 || items[1] != 3 || items[2] != 4)
{
    throw new InvalidOperationException("SlidingWindowBuilder smoke test failed.");
}

var movingAverage = MovingAverageBuilder.Create(3);
movingAverage.Add(1);
movingAverage.Add(2);
movingAverage.Add(3);
if (Math.Abs(movingAverage.CurrentAverage - 2) > Tolerance)
{
    throw new InvalidOperationException("MovingAverageBuilder smoke test failed.");
}

Console.WriteLine("Native AOT smoke test passed.");
