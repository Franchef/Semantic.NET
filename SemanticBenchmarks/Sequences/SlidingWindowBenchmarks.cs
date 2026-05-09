using BenchmarkDotNet.Attributes;
using Semantic.Sequences;

namespace SemanticBenchmarks.Sequences;

[MemoryDiagnoser]
public class SlidingWindowBenchmarks
{
    private ISlidingWindow<double> _window = null!;

    [Params(10, 100, 1000)]
    public int WindowSize { get; set; }

    [Params(100, 10_000)]
    public int ItemCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _window = SlidingWindowBuilder.Create<double>(WindowSize);
    }

    [Benchmark]
    public void Add()
    {
        _window.Clear();
        for (int i = 0; i < ItemCount; i++)
        {
            _window.Add(i);
        }
    }

    [Benchmark]
    public double[] GetItems()
    {
        _window.Clear();
        for (int i = 0; i < WindowSize; i++)
        {
            _window.Add(i);
        }
        return _window.GetItems();
    }

    [Benchmark]
    public double[] AddAndGetItems()
    {
        _window.Clear();
        double[] result = [];
        for (int i = 0; i < ItemCount; i++)
        {
            _window.Add(i);
            result = _window.GetItems();
        }
        return result;
    }
}
