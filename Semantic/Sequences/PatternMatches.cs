using System.Buffers;

namespace Semantic.Sequences;

internal class PatternMatches : IPatternMatches
{
    private readonly int[] _pattern;
    private int _currentIndex = 0;

    public PatternMatches(IEnumerable<int> pattern)
    {
        _pattern = pattern.ToArray();
    }

    public bool HasMatch()
    {
        return _currentIndex == _pattern.Length;
    }

    public void Next(int item)
    {
        if (_currentIndex == _pattern.Length)
        {
            _currentIndex = 0;
        }

        if (item == _pattern[_currentIndex])
        {
            _currentIndex++;
        }
        else
        {
            _currentIndex = 0; // Reset if the sequence breaks
        }
    }
}
