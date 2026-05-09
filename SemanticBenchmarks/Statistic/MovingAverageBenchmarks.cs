using BenchmarkDotNet.Attributes;
using Semantic.Statistic;

namespace SemanticBenchmarks.Statistic;

[MemoryDiagnoser]
public class MovingAverageBenchmarks
{
    private IMovingAverage _movingAverage = null!;
    private IMovingAverage _movingAverageCustomStep = null!;
    private double[] _data = null!;

    [Params(10, 100)]
    public int WindowSize { get; set; }

    [Params(1000, 10_000)]
    public int DataLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var rng = new Random(42);
        _data = new double[DataLength];
        for (int i = 0; i < DataLength; i++)
        {
            _data[i] = rng.NextDouble() * 100.0;
        }
    }

    [Benchmark]
    public double MovingAverageAdd()
    {
        _movingAverage = MovingAverageBuilder.Create(WindowSize);
        foreach (var value in _data)
        {
            _movingAverage.Add(value);
        }
        return _movingAverage.CurrentAverage;
    }

    [Benchmark]
    public double MovingAverageCustomStepAdd()
    {
        _movingAverageCustomStep = MovingAverageBuilder.CreateCustomStep(WindowSize, WindowSize / 2 > 0 ? WindowSize / 2 : 1);
        foreach (var value in _data)
        {
            _movingAverageCustomStep.Add(value);
        }
        return _movingAverageCustomStep.CurrentAverage;
    }
}
