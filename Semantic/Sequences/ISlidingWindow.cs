using System;

namespace Semantic.Sequences;

public interface ISlidingWindow<T>
{
    int Size { get; }

    void Add(T item);

    void Clear();

    T[] GetItems();
}
