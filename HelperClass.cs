using System;
using System.Collections.Generic;
using System.Linq;

public static class TestClass
{
    public static bool TryFindIndices<T>(this IEnumerable<T> items, Func<T, bool> predicate, out IEnumerable<int> indices) 
    {
        int i = 0;
        List<int> indicesList = new List<int>(1);
        foreach (var item in items) 
        {
            if (predicate(item))
            {
                indicesList.Add(i);
            }

            i++;
        }

        indices = indicesList.AsEnumerable();
        return indicesList.Count > 0;
    } //modified from sta ckover flow.c om/questions/14476162/14476244#14476244
}