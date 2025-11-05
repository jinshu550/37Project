using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

[RequireComponent(typeof(VideoPlayer))]
public class VideoFadeController : MonoBehaviour
{
    [Header("淡入淡出设置")]
    public float fadeInDuration = 1.0f;
    public float fadeOutDuration = 1.0f;
    public float startFadeOutBeforeEnd = 2.0f;

    [Header("引用")]
    public Image fadeMask;
    [SerializeField] private Button skipButton;  //跳过按钮
    private VideoPlayer videoPlayer;
    private bool isVideoCompleted = false; // 标记视频是否已完整播放

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // 确保关闭循环播放
        videoPlayer.isLooping = false;

        if (fadeMask == null)
        {
            CreateFadeMask();
        }

    }

    void Start()
    {
        SetMaskAlpha(1.0f);
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoEnded; // 监听视频结束事件
        videoPlayer.Prepare();
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipVideo);
        }
        else
        {
            Debug.LogWarning("请为 VideoFadeController 赋值「skipButton」！");
        }

    }

    void Update()
    {
        // 只有视频未结束时，才判断是否需要淡出
        if (!isVideoCompleted && videoPlayer.isPlaying &&
            videoPlayer.length > 0 &&
            (videoPlayer.length - videoPlayer.time) <= startFadeOutBeforeEnd)
        {
            StartCoroutine(FadeOut());
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        isVideoCompleted = false; // 重置播放状态
        videoPlayer.Play();
        StartCoroutine(FadeIn());
    }

    // 视频播放结束时调用
    private void OnVideoEnded(VideoPlayer vp)
    {
        isVideoCompleted = true; // 标记视频已结束
        // 视频播放结束后，触发事件
        EventHandler.CallOnVideoComplete();
    }

    /// <summary> 跳过按键 </summary>
    public void SkipVideo()
    {
        if (videoPlayer != null && !isVideoCompleted)
        {
            videoPlayer.Stop(); // 停止视频播放
            OnVideoEnded(videoPlayer); // 触发“视频结束”逻辑
            StopAllCoroutines(); // 停止淡入/淡出协程
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeInDuration);
            SetMaskAlpha(alpha);
            yield return null;
        }

        SetMaskAlpha(0.0f);
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0.0f, 1.0f, elapsedTime / fadeOutDuration);
            SetMaskAlpha(alpha);
            yield return null;
        }
        SetMaskAlpha(1.0f);
    }

    private void SetMaskAlpha(float alpha)
    {
        Color color = fadeMask.color;
        color.a = alpha;
        fadeMask.color = color;
    }

    private void CreateFadeMask()
    {
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.AddComponent<Canvas>();
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        GameObject maskObj = new GameObject("FadeMask");
        maskObj.transform.SetParent(canvasObj.transform);
        fadeMask = maskObj.AddComponent<Image>();

        RectTransform rect = maskObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        fadeMask.color = new Color(0, 0, 0, 1);
    }

    void OnDestroy()
    {
        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.loopPointReached -= OnVideoEnded; // 移除事件监听
    }
}
