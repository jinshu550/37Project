using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2Blocker : MonoBehaviour
{
    [Header("场景解锁相关")]
    private GameObject scene2Blocker;
    [SerializeField] private Animator scene2BlockerAnim; // 遮挡物的动画组件
    [SerializeField] private bool hasPlayedUnlock = false; // 标记是否解锁动画
    [SerializeField] private bool isPlayingAnimation = false; // 标记动画是否播放

    private void Awake()
    {
        if (scene2BlockerAnim == null)
            scene2BlockerAnim = gameObject.GetComponentInChildren<Animator>();
        scene2Blocker = gameObject.transform.GetChild(0).gameObject;
    }


    void OnEnable()
    {
        // 监听场景解锁事件
        EventHandler.OnSceneUnlocked += OnSceneUnlocked;
        // 面板激活时，同步一次状态
        SyncStateOnEnable();
    }

    void OnDisable()
    {
        // 监听场景解锁事件
        EventHandler.OnSceneUnlocked -= OnSceneUnlocked;
        if (GameDataManager.Instance.IsSceneUnlocked(2))
        {
            isPlayingAnimation = false;
            scene2Blocker.SetActive(false);
        }
    }

    void Update()
    {
        // 如果是打开地图且场景二已解锁 并解锁动画
        if (scene2Blocker.activeSelf && GameDataManager.Instance.IsSceneUnlocked(2) && !hasPlayedUnlock && !isPlayingAnimation)
        {
            PlayScene2UnlockAnimation();
        }
    }

    /// <summary>面板激活时同步状态（解决重新打开地图的显示问题）</summary>
    private void SyncStateOnEnable()
    {
        if (GameDataManager.Instance.IsSceneUnlocked(2))
        {
            // 已解锁：如果没播放过动画，等待播放；如果已播放，直接隐藏
            if (hasPlayedUnlock && !isPlayingAnimation)
            {
                isPlayingAnimation = false;
                scene2Blocker.SetActive(false);
            }

        }
        else
        {
            // 未解锁：强制显示云
            scene2Blocker.SetActive(true);
            hasPlayedUnlock = false;
            isPlayingAnimation = false;
        }
    }

    private void OnSceneUnlocked(int sceneIndex)
    {
        // 仅当解锁的是场景2时，标记需要播放动画
        if (sceneIndex == 2)
        {
            hasPlayedUnlock = false; // 重置标记，确保动画会播放
        }
    }

    /// <summary>播放场景二解锁动画</summary>
    private void PlayScene2UnlockAnimation()
    {
        if (scene2BlockerAnim == null) return;

        isPlayingAnimation = true;//动画开始播放
        hasPlayedUnlock = true; // 标记为已触发
        scene2BlockerAnim.SetTrigger("FlyAway");// 触发飘走动画

        StartCoroutine(DestroyAfterAnimation());// 等待动画结束后销毁
    }

    /// <summary>等待动画播放完成后销毁遮挡物</summary>
    private IEnumerator DestroyAfterAnimation()
    {
        // 等待动画组件和遮挡物存在
        if (scene2BlockerAnim == null || scene2Blocker == null)
            yield break;
        // 等待动画播放完成（假设动画长度在1秒以内，可根据实际动画调整）
        yield return new WaitForSeconds(1f);

        // 检查是否还有动画在播放
        while (scene2BlockerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        isPlayingAnimation = false;
        // 隐藏遮挡物 等待下次新开游戏时显示
        scene2Blocker.SetActive(false);
    }


}
