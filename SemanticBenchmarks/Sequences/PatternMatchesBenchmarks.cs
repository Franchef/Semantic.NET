using BenchmarkDotNet.Attributes;
using Semantic.Sequences;

namespace SemanticBenchmarks.Sequences;

[MemoryDiagnoser]
public class PatternMatchesBenchmarks
{
    private int[] _pattern = null!;
    private int[] _inputNoMatch = null!;
    private int[] _inputWithMatch = null!;

    [Params(3, 10)]
    public int PatternLength { get; set; }

    [Params(1000, 10_000)]
    public int InputLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _pattern = Enumerable.Range(1, PatternLength).ToArray();

        // Input that never matches (all zeros)
        _inputNoMatch = new int[InputLength];

        // Input that contains the pattern at the end
        _inputWithMatch = new int[InputLength];
        for (int i = 0; i < PatternLength; i++)
        {
            _inputWithMatch[InputLength - PatternLength + i] = _pattern[i];
        }
    }

    [Benchmark]
    public bool FeedNoMatch()
    {
        var matcher = PatternMatchesBuilder.Create(_pattern);
        foreach (var item in _inputNoMatch)
        {
            matcher.Next(item);
        }
        return matcher.HasMatch();
    }

    [Benchmark]
    public bool FeedWithMatch()
    {
        var matcher = PatternMatchesBuilder.Create(_pattern);
        foreach (var item in _inputWithMatch)
        {
            matcher.Next(item);
        }
        return matcher.HasMatch();
    }
}
