using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extend 
{
   public static string GetPathName(this Transform transform,string result)
    {
        if (string.IsNullOrEmpty(result))
        {
            result = transform.name;
        }
        else
        {
            result = transform.name + "/" + result;
        }
        if (transform.parent == null)
            return result;
        return GetPathName(transform.parent, result);
    }
}
