namespace Semantic.Sequences;

/// <summary>
/// This class implements a sliding window of a fixed size. When the window is full, adding a new item will remove the oldest item from the window. The GetItems method returns the current items in the window in the order they were added, with the oldest item first and the newest item last.
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class SlidingWindow<T> : ISlidingWindow<T>
{
    public SlidingWindow(int size)
    {
        if(size < 1)
        {
            throw new ArgumentException("Size must be greater than 0.", nameof(size));
        }
        Size = size;
        _items = new T[size];
    }
    private readonly T[] _items;
    private int _index = -1;
    private bool _isSliding = false;

    public int Size { get; }

    public void Add(T item)
    {
        _index++;
        if(_index >= Size)
        {
            _index = 0;
            _isSliding = true;
        }
        _items[_index] = item;
    }

    public void Clear()
    {
        Array.Clear(_items);
        _index = -1;
        _isSliding = false;
    }
    public T[] GetItems()
    {
        if(_index == -1)
        {
            return Array.Empty<T>();
        }

        if(_isSliding)
        {
            var result = new T[Size];
            var oldestIndex = (_index + 1) % Size;

            Array.Copy(_items, oldestIndex, result, 0, Size - oldestIndex);
            if(oldestIndex > 0)
            {
                Array.Copy(_items, 0, result, Size - oldestIndex, oldestIndex);
            }
            return result;
        } else {
            var partialWindow = new T[_index + 1];
            Array.Copy(_items, 0, partialWindow, 0, _index + 1);
            return partialWindow;
        }
    }
}