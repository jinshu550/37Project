using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class GlobalSceneFader : ExploreSingleton<GlobalSceneFader>
{
    [Header("过渡效果设置")]
    [Tooltip("淡入动画持续时间（秒）")]
    [SerializeField] private float fadeInDuration = 0.3f;

    [Tooltip("淡出动画持续时间（秒）")]
    [SerializeField] private float fadeOutDuration = 0.3f;

    [Tooltip("加载场景名称（用于识别）")]
    [SerializeField] private string loaderSceneName = "Loader_Scene";  // 加载场景名称

    [Tooltip("过渡遮罩颜色")]
    [SerializeField] private Color fadeColor = Color.white;

    [Header("UI引用")]
    [SerializeField] private Image fadeMask;  // 可以在Inspector指定，也可以自动创建

    private Canvas fadeCanvas;
    private GameObject canvasObj;
    private bool isFading = false;  // 防止同时触发多个过渡动画

    protected override void Awake()
    {
        base.Awake();  // 调用基类单例逻辑

        // 初始化过渡UI
        InitializeFadeUI();

    }

    void OnEnable()
    {
        // 订阅场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 取消订阅，防止内存泄漏
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 初始化淡入淡出UI组件
    /// </summary>
    private void InitializeFadeUI()
    {
        // 如果没有指定遮罩，则自动创建
        if (fadeMask == null)
        {
            CreateFadeUI();
        }
        else
        {
            fadeCanvas = fadeMask.GetComponentInParent<Canvas>();
            if (fadeCanvas == null)
            {
                // 为现有遮罩创建画布
                canvasObj = new GameObject("FadeCanvas");
                canvasObj.transform.SetParent(transform);
                fadeCanvas = canvasObj.AddComponent<Canvas>();
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                fadeMask.transform.SetParent(canvasObj.transform);
            }
        }

        // 确保画布设置正确
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 10;  // 确保在最上层

        //确保遮罩不会挡着鼠标的识别
        fadeMask.raycastTarget = false;

        // 初始状态设为全白（准备淡入）
        SetFadeAlpha(1f);
    }

    /// <summary>
    /// 自动创建淡入淡出所需的UI组件
    /// </summary>
    private void CreateFadeUI()
    {
        // 创建画布
        canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform);
        fadeCanvas = canvasObj.AddComponent<Canvas>();
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建遮罩
        GameObject maskObj = new GameObject("FadeMask");
        maskObj.transform.SetParent(canvasObj.transform);
        fadeMask = maskObj.AddComponent<Image>();

        // 设置遮罩尺寸为全屏
        RectTransform rect = maskObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // 设置遮罩颜色
        fadeMask.color = fadeColor;
    }

    /// <summary>
    /// 设置遮罩的透明度
    /// </summary>
    private void SetFadeAlpha(float alpha)
    {
        Color color = fadeMask.color;
        color.a = alpha;
        fadeMask.color = color;
    }

    /// <summary>
    /// 场景加载完成后触发淡入效果
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 根据场景名称判断是否为加载场景
        if (scene.name != loaderSceneName)
        {
            StartCoroutine(FadeIn());
        }
    }

    /// <summary> 淡入协程：从白色过渡到透明 </summary>
    private IEnumerator FadeIn()
    {
        canvasObj.SetActive(true);
        if (isFading) yield break;
        isFading = true;

        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(0f);  // 确保完全透明
        isFading = false;
    }

    /// <summary>
    /// 淡出协程：从透明过渡到白色
    /// </summary>
    private IEnumerator FadeOut(Action onComplete)
    {
        if (isFading) yield break;
        isFading = true;

        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }
        SetFadeAlpha(1f);  // 确保完全白色
        isFading = false;
        onComplete?.Invoke();  // 通知淡出完成

    }

    /// <summary>
    /// 外部调用：带过渡效果加载场景
    /// </summary>
    /// <param name="sceneIndex">场景索引</param>
    public void LoadSceneWithFade()
    {
        // 获取加载场景索引
        int loaderSceneIndex = GetSceneIndexByName(loaderSceneName);
        if (loaderSceneIndex == -1)
        {
            Debug.LogError($"加载场景「{loaderSceneName}」未在Build Settings中！");
            return;
        }
        //SceneManager.LoadScene(loaderSceneIndex);
        // 执行淡出后，直接加载加载场景
        StartCoroutine(FadeOut(() =>
        {
            
            SceneManager.LoadScene(loaderSceneName);
        }));
    }

    /// <summary> 带过渡效果加载场景
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    // public void LoadSceneWithFade(string sceneName)
    // {
    //     int sceneIndex = GetSceneIndexByName(sceneName);
    //     if (sceneIndex == -1)
    //     {
    //         Debug.LogError($"场景 {sceneName} 不在Build Settings中！");
    //         return;
    //     }

    //     LoadSceneWithFade(sceneIndex);
    // }

    /// <summary>
    /// 根据场景名称获取索引
    /// </summary>
    public int GetSceneIndexByName(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
            {
                return i;
            }
        }
        return -1;
    }
}
