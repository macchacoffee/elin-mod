using System;
using System.Collections;
using System.Collections.Generic;

namespace StarweaversMoongateQuickFix.Mod;

internal sealed class ModPagedCollection<T>(List<T> source, int pageSize) : ICollection<T>
{
    private readonly List<T> _source = source;
    private readonly int _pageSize = pageSize;

    public int Page { get; private set; }

    public int PageCount => Math.Max(1, (_source.Count + _pageSize - 1) / _pageSize);

    public int Count
    {
        get
        {
            NormalizePage();
            var start = Page * _pageSize;
            return Math.Min(_pageSize, Math.Max(0, _source.Count - start));
        }
    }

    public bool IsReadOnly => true;

    public void NextPage()
    {
        Page = (Page + 1) % PageCount;
    }

    public void PrevPage()
    {
        Page--;
        if (Page < 0)
        {
            Page = PageCount - 1;
        }
    }

    public void NormalizePage()
    {
        if (Page >= PageCount)
        {
            Page = PageCount - 1;
        }
        if (Page < 0)
        {
            Page = 0;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        NormalizePage();

        var start = Page * _pageSize;
        var end = Math.Min(start + _pageSize, _source.Count);
        for (var i = start; i < end; i++)
        {
            yield return _source[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Contains(T item)
    {
        foreach (var current in this)
        {
            if (EqualityComparer<T>.Default.Equals(current, item))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public void Add(T item)
    {
        throw new NotSupportedException();
    }

    public bool Remove(T item)
    {
        throw new NotSupportedException();
    }

    public void Clear()
    {
        throw new NotSupportedException();
    }
}
