using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    private static Game instance;
    private static System.Object go = new System.Object();

    public static Game GetInstance()
    {
        lock (go)
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Game");
                instance = go.AddComponent<Game>();
                DontDestroyOnLoad(go);
            }

            return instance;
        }
    }

    void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        MouseUtil.OnSceneExit();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        MouseUtil.OnSceneLoaded();
    }

    public void ChangeScene()
    {

    }
}
