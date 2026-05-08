using System;

namespace Semantic.Sequences;

public interface ISlidingWindow<T>
{
    public int Size { get; }

    public void Add(T item);

    public void Clear();

    public T[] GetItems();
}
