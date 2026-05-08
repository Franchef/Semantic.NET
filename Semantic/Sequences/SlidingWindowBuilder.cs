namespace Semantic.Sequences;

public static class SlidingWindowBuilder
{
    public static ISlidingWindow<T> Create<T>(int size) => new SlidingWindow<T>(size);
}