using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
public static class Loader
{
    private static string targetScene; // 存储目标场景名称

    public static void Load(string targetSceneName)
    {
        targetScene = targetSceneName;
        SceneManager.LoadScene("Loader_Scene"); //加载
    }

    public static string ReturnToTargetScene()
    {
        return targetScene;
    }

    //手动设置目标场景
    public static void SetTargetScene(string targetSceneName)
    {
        targetScene = targetSceneName;
        GlobalSceneFader.Instance.LoadSceneWithFade();
    }


}
// public enum Load_Scene
// {
//     MainMenu,
//     Loader_Scene,
//     Explore_001Scene,
//     Explore_002Scene,
//     Fight_Scene
// }
