namespace Semantic.Sequences;

public interface IPatternMatches<T>
{
    bool HasMatch();
    void Next(T item);

    /// <summary>
    /// Raised when the pattern is matched.
    /// </summary>
    event EventHandler? Matched;
}
