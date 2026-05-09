using BenchmarkDotNet.Attributes;
using Semantic.Sequences;

namespace SemanticBenchmarks.Sequences;

[MemoryDiagnoser]
public class PatternMatchesBenchmarks
{
    private IPatternMatches<int> _matcher = null!;
    private int[] _inputNoMatch = null!;
    private int[] _inputWithMatch = null!;

    [Params(3, 10)]
    public int PatternLength { get; set; }

    [Params(1000, 10_000)]
    public int InputLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var pattern = Enumerable.Range(1, PatternLength).ToArray();
        _matcher = PatternMatchesBuilder.Create(pattern);

        // Input that never matches (all zeros)
        _inputNoMatch = new int[InputLength];

        // Input that contains the pattern at the end
        _inputWithMatch = new int[InputLength];
        for (int i = 0; i < PatternLength; i++)
        {
            _inputWithMatch[InputLength - PatternLength + i] = pattern[i];
        }
    }

    [Benchmark]
    public bool FeedNoMatch()
    {
        foreach (var item in _inputNoMatch)
        {
            _matcher.Next(item);
        }
        return _matcher.HasMatch();
    }

    [Benchmark]
    public bool FeedWithMatch()
    {
        foreach (var item in _inputWithMatch)
        {
            _matcher.Next(item);
        }
        return _matcher.HasMatch();
    }
}
