namespace Semantic.Sequences;

public interface IPatternMatches
{
    bool HasMatch();
    void Next(int item);
}
