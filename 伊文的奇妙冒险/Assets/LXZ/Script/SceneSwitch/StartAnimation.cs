using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartAnimation : MonoBehaviour
{
    [SerializeField] private float dwellTime = 1f;
    [SerializeField] private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        //是否是游戏刚启动（同一次运行中首次加载）
        if (GameStartup.Instance != null && !GameStartup.Instance.IsStartAnimationPlayed)
        {
            // 首次启动：播放动画
            StartCoroutine(FadeImage(1, 0));
        }
        else
        {
            // 非首次启动（如场景切换后）：直接销毁，不播放
            Destroy(gameObject);
        }
    }

    private IEnumerator FadeImage(float startAlpha, float targetAlpha)
    {
        yield return new WaitForSeconds(3f);

        if (image == null) yield break; // 安全判断

        float elapsed = 0f; // 已流逝时间
        Color currentColor = image.color; // 缓存当前颜色

        while (elapsed < dwellTime)
        {
            elapsed += Time.deltaTime; // 累加时间
            // 计算当前透明度（Lerp：线性插值，从startAlpha到targetAlpha）
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / dwellTime);
            // 实时更新颜色的alpha值
            currentColor.a = currentAlpha;
            image.color = currentColor;
            yield return null; // 等待一帧，实现平滑过渡
        }

        // 渐变结束后，确保最终透明度准确
        currentColor.a = targetAlpha;
        image.color = currentColor;

        // 标记动画已播放
        GameStartup.Instance?.MarkAnimationPlayed();
        // 销毁动画对象
        Destroy(gameObject);
    }
}
