using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtension 
{
    //从列表中取出一张牌并且返回
    public static T Draw<T>(this List<T> list) 
    {
        if (list.Count == 0) return default;
        int r = Random.Range(0, list.Count);
        T t = list[r];
        list.Remove(t);
        return t;       
    }
}
