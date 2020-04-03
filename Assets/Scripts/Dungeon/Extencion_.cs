using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extencion_
{
    public static bool Has<T>(this IList<T> l , T item)
    {
        return l.IndexOf(item) != -1;
    }
}
