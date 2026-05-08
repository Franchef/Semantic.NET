using System.Buffers;

namespace Semantic.Sequences;

internal class PatternMatches<T> : IPatternMatches<T>
{
    private readonly T[] _pattern;
    private int _currentIndex = 0;

    public event EventHandler? Matched;

    public PatternMatches(IEnumerable<T> pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        _pattern = pattern.ToArray();
        if (_pattern.Length == 0)
            throw new ArgumentException("Pattern must not be empty.", nameof(pattern));
    }

    public bool HasMatch()
    {
        return _currentIndex == _pattern.Length;
    }

    public void Next(T item)
    {
        if (_currentIndex == _pattern.Length)
        {
            _currentIndex = 0;
        }

        if (EqualityComparer<T>.Default.Equals(_pattern[_currentIndex], item))
        {
            _currentIndex++;
            if (_currentIndex == _pattern.Length)
            {
                Matched?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            _currentIndex = 0; // Reset if the sequence breaks
        }
    }
}
