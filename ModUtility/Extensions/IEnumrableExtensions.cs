using System;
using System.Collections.Generic;
using System.Linq;

namespace ModUtility.Extensions;

public static class IEnumrableExtensions
{
    public static IEnumerable<IEnumerable<TSource>> Chunk<TSource>(this IEnumerable<TSource> source, int size)
    {
        if (size <= 0)
        {
            throw new ArgumentException("Chuck size must be more than 0");
        }

        while(source.Any())
        {
            yield return source.Take(size);
            source = source.Skip(size);
        }
    }
}