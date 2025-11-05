using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventHandler
{
    #region 背包相关事件
    // 背包UI更新事件
    public static event Action<List<CardBasicInformation>> UpdateInventoryUI;
    /// <summary> 背包UI更新事件
    /// </summary>
    /// <param name="list">背包的列表更新</param>
    public static void CallUpdateInventoryUI(List<CardBasicInformation> list)
    {

        UpdateInventoryUI?.Invoke(list);
    }

    public static event Action<bool, BackCardUI, List<BackCardUI>> OnSelectionChanged;
    /// <summary> 触发选择变化事件
    /// </summary>
    /// <param name="isSingleMode">是否单选模式</param>
    /// <param name="singleCard">单选选中的卡牌</param>
    /// <param name="multiCards">多选选中的卡牌</param>
    public static void CallOnSelectionChanged(bool isSingleMode, BackCardUI singleCard, List<BackCardUI> multiCards)
    {
        OnSelectionChanged?.Invoke(isSingleMode, singleCard, new List<BackCardUI>(multiCards)); // 传递副本避免外部修改
    }

    public static event Action<Color> OnDescriptionColorChanged;
    /// <summary> 用于背包触发颜色变更的方法 </summary>
    /// <param name="color"></param>
    public static void CallOnDescriptionColorChanged(Color color)
    {
        OnDescriptionColorChanged?.Invoke(color);
    }

    // 合成请求事件（BackpackUIManager -> CardSynthesisManager）
    public static event Action<List<BackCardUI>> OnSynthesisRequested;
    /// <summary>触发合成请求（传递选中的卡牌列表）</summary>
    public static void CallOnSynthesisRequested(List<BackCardUI> selectedCards)
    {
        OnSynthesisRequested?.Invoke(selectedCards);
    }

    //合成结果事件（CardSynthesisManager -> SelectionShowUI）
    public static event Action<CardBasicInformation> OnSynthesisResulted;
    /// <summary> 触发合成结果（传递生成的卡牌预设体）
    /// </summary>
    /// <param name="targetCard">找到合成的卡牌</param>
    public static void CallOnSynthesisResulted(CardBasicInformation targetCard)
    {
        OnSynthesisResulted?.Invoke(targetCard);
    }

    public static event Action OnSynthesisFailed;
    /// <summary> 合成失败事件 </summary>
    public static void CallOnSynthesisFailed()
    {
        OnSynthesisFailed?.Invoke();
    }

    public static event Action OnSelectionCleared;
    /// <summary> 选择切换/清除事件 </summary>
    public static void CallOnSelectionCleared()
    {
        OnSelectionCleared?.Invoke();
    }

    /// <summary> 物品收集成功后触发收集弹窗 </summary>
    public static event Action<string> OnItemCollected;
    public static void CallOnItemCollected(string itemName) => OnItemCollected?.Invoke(itemName);

    #endregion

    /// <summary> 物品缺少后触发提示弹窗 </summary>
    public static event Action<int> OnLackItem;
    public static void CallOnLackItem(int itemID) => OnLackItem?.Invoke(itemID);


    public static event Action<bool> OnDialogueStateChanged;
    /// <summary> 是否锁定输入（玩家输入，按钮（背包，地图，设置）输入）
    /// </summary>
    /// <param name="isDialogueActive">true订阅锁定，false订阅解锁</param>
    public static void CallOnDialogueStateChanged(bool isDialogueActive)
    {
        OnDialogueStateChanged?.Invoke(isDialogueActive);
    }

    //音频 AudioManager初始化完成事件
    public static event Action OnAudioManagerInited;
    /// <summary> 音频初始化完成后触发事件，通知SettingPanel可以加载设置 </summary>
    public static void CallAudioManagerInited()
    {
        OnAudioManagerInited?.Invoke();
    }

    //音频分组调整
    public static event Action<string, float> OnGroupVolumeChanged;
    /// <summary>触发分组音量变化（由SettingPanel调用，通知AudioManager更新音量） </summary>
    public static void CallOnGroupVolumeChanged(string groupName, float volume)
    {
        OnGroupVolumeChanged?.Invoke(groupName, volume);
    }

    // 场景解锁事件
    public static event Action<int> OnSceneUnlocked;
    public static void CallOnSceneUnlocked(int sceneIndex) => OnSceneUnlocked?.Invoke(sceneIndex);

    // 背包解锁事件
    public static event Action OnBackpackUnlocked;
    public static void CallOnBackpackUnlocked() => OnBackpackUnlocked?.Invoke();

    // 所有数据重置事件（用于刷新UI状态）
    public static event Action OnAllDataReset;
    public static void CallOnAllDataReset() => OnAllDataReset?.Invoke();


    #region 场景过渡事件
    public static event Action OnVideoComplete;
    /// <summary> 过渡场景动画结束
    /// </summary>
    /// <param name="count"></param>
    public static void CallOnVideoComplete()
    {
        OnVideoComplete?.Invoke();
    }

    /// <summary> 触发场景淡出 </summary>
    public static event Action<int> OnSceneFadeOutStart;

    /// <summary> 触发场景淡出（外部调用） </summary>
    public static void CallSceneFadeOutStart(int targetSceneName)
    {
        OnSceneFadeOutStart?.Invoke(targetSceneName);
    }

    #endregion

}
