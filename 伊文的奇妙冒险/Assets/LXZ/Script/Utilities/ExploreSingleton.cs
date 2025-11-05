using UnityEngine;

public class ExploreSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            transform.parent = null;
            DontDestroyOnLoad(gameObject); // 跨场景不销毁
        }
        else
        {
            Destroy(gameObject); // 防止重复创建
        }
    }
}