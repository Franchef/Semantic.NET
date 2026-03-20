using System;

namespace Semantic.Sequences;

public static class PatternMatchesBuilder
{
    public static Builder StartsWith(int firstElement)
    {
        return new Builder(firstElement);
    }
    public sealed class Builder
    {
        IList<int> _matches = new List<int>();
        internal Builder(int firstElement)
        {
            _matches.Add(firstElement);
        }

        public Builder ContinuesWith(int nextElement)
        {
            _matches.Add(nextElement);
            return this;
        }

        public IPatternMatches EndsWith(int lastElement)
        {
            _matches.Add(lastElement);
            return new PatternMatches(_matches);
        }
    }
}
