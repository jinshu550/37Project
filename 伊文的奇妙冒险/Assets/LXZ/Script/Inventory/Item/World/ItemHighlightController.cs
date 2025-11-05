using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHighlightController : MonoBehaviour
{
    [Header("基础配置")]
    [SerializeField] private string playerTag = "Player"; // 玩家标签（判断触发者）
    [SerializeField] private GameObject GlowPrefabs;

    [Header("解锁监听配置")]
    [SerializeField] private UnlockType unlockType;       // 监听的解锁类型（背包/关卡2等）


    private bool _isUnlocked;           // 当前物品是否已解锁


    private void Awake()
    {
        // 初始化解锁状态（从GameDataManager读取）
        UpdateUnlockState();
    }


    private void OnEnable()
    {
        // 监听数据重置事件（新游戏时重新检查解锁状态）
        EventHandler.OnAllDataReset += UpdateUnlockState;
    }


    private void OnDisable()
    {
        // 取消事件订阅（防止内存泄漏）
        EventHandler.OnAllDataReset -= UpdateUnlockState;
    }

    void Update()
    {
        //在解锁前会检测什么时候会解锁
        if (!_isUnlocked)
        {
            UpdateUnlockState();
        }
    }


    /// <summary> 更新当前物品的解锁状态（根据监听类型） </summary>
    private void UpdateUnlockState()
    {
        if (GameDataManager.Instance == null)
        {
            _isUnlocked = false;
            Debug.LogWarning("GameDataManager实例未找到，默认未解锁！");
            return;
        }

        // 根据解锁类型，读取对应的数据键状态
        switch (unlockType)
        {
            case UnlockType.Backpack:
                _isUnlocked = GameDataManager.Instance.HasBackpackUnlocked();
                break;
            case UnlockType.Scene2:
                _isUnlocked = GameDataManager.Instance.IsSceneUnlocked(2);
                break;
            case UnlockType.FunctionCardCollect:
            default:
                _isUnlocked = false;
                break;
        }

        // 若已解锁，强制隐藏发光特效（避免解锁后仍显示高亮）
        if (_isUnlocked && GlowPrefabs != null)
        {

            GlowPrefabs.SetActive(false);
        }
    }


    #region 触发器逻辑
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 仅响应玩家，且物品未解锁
        if (other.CompareTag(playerTag) && !_isUnlocked)
        {
            if (GlowPrefabs != null)
                GlowPrefabs.SetActive(true);

        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        // 玩家离开，且物品未解锁
        if (other.CompareTag(playerTag) && !_isUnlocked)
        {
            // _isPlayerInRange = false;
            if (GlowPrefabs != null)
                GlowPrefabs.SetActive(false);
        }
    }
    #endregion




    /// <summary> 解锁类型枚举（可根据需求扩展） </summary>
    public enum UnlockType
    {
        [Tooltip("监听背包解锁（HasBackpack键）")]
        Backpack,
        [Tooltip("监听关卡2解锁（Scene2Unlocked键）")]
        Scene2,
        [Tooltip("功能卡牌收集")]
        FunctionCardCollect
    }
}