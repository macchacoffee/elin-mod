using System;
using System.Collections;
using System.Collections.Generic;

namespace Macchacoffee.ElinMods.StarweaversMoongatePaging.Mod;

internal sealed class PagedCollection<T>(List<T> source, int pageSize) : ICollection<T>
{
    private readonly List<T> _source = source ?? throw new ArgumentNullException(nameof(source));
    private readonly int _pageSize = pageSize > 0
        ? pageSize
        : throw new ArgumentOutOfRangeException(nameof(pageSize));

    public int PageIndex { get; private set; }

    public int PageCount => _source.Count == 0 ? 1 : ((_source.Count - 1) / _pageSize) + 1;

    public bool CanMovePrevious => PageIndex > 0;

    public bool CanMoveNext => PageIndex < PageCount - 1;

    public int Count
    {
        get
        {
            ClampPageIndex();
            var start = PageIndex * _pageSize;
            return Math.Min(_pageSize, Math.Max(0, _source.Count - start));
        }
    }

    public bool IsReadOnly => true;

    public bool MoveNext()
    {
        ClampPageIndex();
        if (!CanMoveNext)
        {
            return false;
        }

        PageIndex++;
        return true;
    }

    public bool MovePrevious()
    {
        ClampPageIndex();
        if (!CanMovePrevious)
        {
            return false;
        }

        PageIndex--;
        return true;
    }

    public void ClampPageIndex()
    {
        if (PageIndex >= PageCount)
        {
            PageIndex = PageCount - 1;
        }
        if (PageIndex < 0)
        {
            PageIndex = 0;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        ClampPageIndex();

        var start = PageIndex * _pageSize;
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
