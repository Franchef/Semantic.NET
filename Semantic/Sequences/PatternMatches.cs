namespace Semantic.Sequences;

internal class PatternMatches<T> : IPatternMatches<T>
{
    private readonly T[] _pattern;
    private readonly object _syncRoot = new();
    private readonly int[] _longestPrefixSuffix;
    private int _currentIndex = 0;
    private bool _hasMatch;

    public event EventHandler? Matched;

    public PatternMatches(IEnumerable<T> pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        _pattern = pattern.ToArray();
        if (_pattern.Length == 0)
            throw new ArgumentException("Pattern must not be empty.", nameof(pattern));
        _longestPrefixSuffix = BuildLongestPrefixSuffix(_pattern);
    }

    public bool HasMatch()
    {
        lock (_syncRoot)
        {
            return _hasMatch;
        }
    }

    public void Next(T item)
    {
        EventHandler? matched = null;

        lock (_syncRoot)
        {
            _hasMatch = false;

            while (
                _currentIndex > 0 &&
                !EqualityComparer<T>.Default.Equals(_pattern[_currentIndex], item)
            )
            {
                _currentIndex = _longestPrefixSuffix[_currentIndex - 1];
            }

            if (EqualityComparer<T>.Default.Equals(_pattern[_currentIndex], item))
            {
                _currentIndex++;
            }

            if (_currentIndex == _pattern.Length)
            {
                _hasMatch = true;
                matched = Matched;
                _currentIndex = _longestPrefixSuffix[_currentIndex - 1];
            }
        }

        matched?.Invoke(this, EventArgs.Empty);
    }

    private static int[] BuildLongestPrefixSuffix(T[] pattern)
    {
        var result = new int[pattern.Length];
        var length = 0;

        for (var i = 1; i < pattern.Length;)
        {
            if (EqualityComparer<T>.Default.Equals(pattern[i], pattern[length]))
            {
                length++;
                result[i] = length;
                i++;
            }
            else if (length > 0)
            {
                length = result[length - 1];
            }
            else
            {
                result[i] = 0;
                i++;
            }
        }

        return result;
    }
}
