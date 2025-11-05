using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MouseUtil
{
    private static Camera camera = Camera.main;
    public static Vector3 GetMousePositionInWorldSpace(int zValue)
    {
        Plane plane = new(camera.transform.forward, new Vector3(0, 0, zValue));
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    public static void OnSceneExit()
    {
        camera = null;
    }

    public static void OnSceneLoaded()
    {
        camera = Camera.main;
    }
}
