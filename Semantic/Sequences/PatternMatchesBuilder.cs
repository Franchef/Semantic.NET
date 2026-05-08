namespace Semantic.Sequences;

public static class PatternMatchesBuilder
{
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
