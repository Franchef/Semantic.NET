namespace Semantic.Sequences;

public static class PatternMatchesBuilder
{
    public static IPatternMatches<T> Create<T>(params T[] pattern)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        if (pattern.Length == 0)
            throw new ArgumentException("Pattern must not be empty.", nameof(pattern));
        return new PatternMatches<T>(pattern);
    }
    public static Builder<T> StartsWith<T>(T firstElement)
    {
        return new Builder<T>(firstElement);
    }
    public sealed class Builder<T>
    {
        IList<T> _matches = new List<T>();
        internal Builder(T firstElement)
        {
            _matches.Add(firstElement);
        }

        public Builder<T> ContinuesWith(T nextElement)
        {
            _matches.Add(nextElement);
            return this;
        }

        public IPatternMatches<T> EndsWith(T lastElement)
        {
            _matches.Add(lastElement);
            return new PatternMatches<T>(_matches);
        }
    }
}
