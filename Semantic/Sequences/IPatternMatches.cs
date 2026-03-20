namespace Semantic.Sequences;

public interface IPatternMatches
{
    bool HasMatch();
    void Next(int item);

    /// <summary>
    /// Raised when the pattern is matched.
    /// </summary>
    event EventHandler? Matched;
}
