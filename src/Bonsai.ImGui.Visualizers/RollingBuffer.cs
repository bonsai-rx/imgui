using System;
using System.Collections;
using System.Collections.Generic;

namespace Bonsai.ImGui.Visualizers;

class RollingBuffer<T>(int capacity) : IReadOnlyCollection<T>
{
    private readonly T[] buffer = new T[capacity];
    private int start;
    private int end;
    private int count;

    public ref T this[int index]
    {
        get => ref buffer[(start + index) % buffer.Length];
    }

    public int Count => count;

    public void Push(T item)
    {
        buffer[end] = item;
        end = (end + 1) % buffer.Length;
        if (count == buffer.Length)
            start = (start + 1) % buffer.Length;
        else
            count++;
    }

    static void CopyList<TSource>(IReadOnlyList<TSource> source, int index, TSource[] buffer, int bufferIndex, int count)
    {
        if (source is TSource[] array)
            Array.Copy(array, index, buffer, bufferIndex, count);
        else if (source is List<TSource> list)
            list.CopyTo(index, buffer, bufferIndex, count);
        else
            for (int i = index, bi = bufferIndex; i < count; i++, bi++)
                buffer[bi] = source[i];
    }

    public void Push(IReadOnlyList<T> items)
    {
        var nextIndex = end + items.Count;
        var trimOffset = (items.Count / buffer.Length) * buffer.Length;
        start = (start + items.Count - Math.Min(items.Count, buffer.Length - count)) % buffer.Length;
        count = Math.Min(count + items.Count, buffer.Length);

        var index = end;
        end = nextIndex % buffer.Length;
        if (end <= index)
        {
            CopyList(items, trimOffset, buffer, index, buffer.Length - index);
            trimOffset += buffer.Length - index;
            index = 0;
        }

        CopyList(items, trimOffset, buffer, index, items.Count - trimOffset);
    }

    public void Push(IEnumerable<T> items)
    {
        if (items is IReadOnlyList<T> list)
            Push(list);
        else
            foreach (var item in items)
                Push(item);
    }

    public void CopyTo(T[] array)
    {
        var startSegmentLength = buffer.Length - start;
        Array.Copy(buffer, start, array, 0, startSegmentLength);
        if (startSegmentLength < buffer.Length)
            Array.Copy(buffer, 0, array, startSegmentLength, buffer.Length - startSegmentLength);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < count; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
