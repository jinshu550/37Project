using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : ExploreSingleton<GameDataManager>
{
    // 定义所有数据键（集中管理，避免硬编码）
    public static class DataKeys
    {
        public const string HasBackpack = "HasBackpack";
        public const string Scene2Unlocked = "Scene2Unlocked";
        public const string CurrentGameScene = "CurrentGameScene";
        public const string BgmVolume = "Setting_BGMVolume";
        public const string SfxVolume = "Setting_SFXVolume";
        public const string Brightness = "Setting_Brightness";
        public const string PlayerPosPrefix = "PlayerPos_"; // 拼接场景名作为唯一键
        public const string CinematicPlayCount = "Cinematic_PlayCount";
        public const string BattleCardIds = "Battle_CardIds";
        public const string TempSavedBoundName = "TempSavedBoundName";

        public const string ElvenUndeadFirstMeeting = "ElvenUndeadFirstMeeting"; //精灵亡者是否首次触发

    }

    // 所有游戏数据键列表（用于全量重置）
    private static readonly List<string> AllDataKeys = new List<string>
    {
        DataKeys.HasBackpack,
        DataKeys.Scene2Unlocked,
        DataKeys.CurrentGameScene,
        DataKeys.BgmVolume,
        DataKeys.SfxVolume,
        DataKeys.Brightness,
        DataKeys.CinematicPlayCount,
        DataKeys.BattleCardIds,
        DataKeys.TempSavedBoundName,
        DataKeys.ElvenUndeadFirstMeeting
    };

    [Header("探索场景配置（用于打包后坐标清理）")]
    [SerializeField]
    private List<string> allExploreSceneNames = new List<string>
    {
        "Explore_001Scene",
        "Explore_002Scene" // 根据项目实际场景名补充
    };

    //当前过场动画计数（内存缓存，提升性能）
    private int _currentCinematicCount = -1;
    //出战列表 缓存
    private List<string> _battleCardIds = new List<string>();
    private int maxBattleCards = 3;  // 最大出战个数

    protected override void Awake()
    {
        base.Awake();
        // 确保数据管理类全局唯一
        DontDestroyOnLoad(gameObject);

        // 初始化时读取过场动画计数
        LoadCinematicCount();

        //初始化时从PlayerPrefs读取出战卡牌ID，加载到内存
        LoadBattleCardIds();
    }

    #region 过场动画跟踪相关
    /// <summary> 加载过场动画播放次数  </summary>
    private void LoadCinematicCount()
    {
        _currentCinematicCount = PlayerPrefs.GetInt(DataKeys.CinematicPlayCount, 0);
    }

    /// <summary>
    /// 获取当前过场动画播放次数
    /// </summary>
    public int GetCinematicPlayCount()
    {
        return _currentCinematicCount;
    }

    /// <summary>
    /// 判断是否是第一次播放过场动画
    /// </summary>
    public bool IsFirstCinematic()
    {
        return _currentCinematicCount == 0;
    }

    /// <summary>
    /// 增加过场动画播放次数（播放后调用）
    /// </summary>
    public void IncrementCinematicCount()
    {
        _currentCinematicCount++;
        PlayerPrefs.SetInt(DataKeys.CinematicPlayCount, _currentCinematicCount);
        PlayerPrefs.Save();

    }

    #endregion

    #region 场景解锁相关
    /// <summary> 标记场景为已解锁 </summary>
    public void UnlockScene(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 2:
                PlayerPrefs.SetInt(DataKeys.Scene2Unlocked, 1);
                PlayerPrefs.Save();
                // 触发场景解锁事件（通知UI更新）
                EventHandler.CallOnSceneUnlocked(sceneIndex);
                break;
                // 可扩展其他场景
        }
    }

    /// <summary> 检查场景是否已解锁 </summary>
    public bool IsSceneUnlocked(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 2:
                return PlayerPrefs.GetInt(DataKeys.Scene2Unlocked, 0) == 1;
            default:
                return false; // 其他场景默认未解锁
        }
    }

    #endregion

    #region 背包相关
    /// <summary> 标记背包为已解锁 </summary>
    public void UnlockBackpack()
    {
        // 只有当背包未解锁时才执行操作并触发事件
        if (!HasBackpackUnlocked())
        {
            PlayerPrefs.SetInt(DataKeys.HasBackpack, 1);
            PlayerPrefs.Save();
            EventHandler.CallOnBackpackUnlocked(); // 仅在状态变更时触发
        }
    }

    /// <summary> 检查背包是否已解锁 </summary>
    public bool HasBackpackUnlocked()
    {
        return PlayerPrefs.GetInt(DataKeys.HasBackpack, 0) == 1;
    }
    #endregion

    #region 场景进度相关
    /// <summary> 保存当前场景进度 </summary>
    public void SaveCurrentScene(string sceneName)
    {
        PlayerPrefs.SetString(DataKeys.CurrentGameScene, sceneName);
        PlayerPrefs.Save();
    }

    /// <summary> 获取保存的场景名称 </summary>
    public string GetSavedSceneName()
    {
        return PlayerPrefs.GetString(DataKeys.CurrentGameScene, string.Empty);
    }
    #endregion

    #region 配置项相关（音量、亮度等）
    /// <summary> 保存音量设置 </summary>
    public void SaveVolumeSettings(float bgmVolume, float sfxVolume)
    {
        PlayerPrefs.SetInt(DataKeys.BgmVolume, Mathf.RoundToInt(bgmVolume));
        PlayerPrefs.SetInt(DataKeys.SfxVolume, Mathf.RoundToInt(sfxVolume));
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 获取保存的音量设置（0-1范围）
    /// </summary>
    public (float bgm, float sfx) GetSavedVolumes()
    {
        float bgm = PlayerPrefs.GetInt(DataKeys.BgmVolume, 50);
        float sfx = PlayerPrefs.GetInt(DataKeys.SfxVolume, 50);
        return (bgm, sfx);
    }

    /// <summary> 保存亮度设置 </summary>
    public void SaveBrightness(int brightness)
    {
        PlayerPrefs.SetInt(DataKeys.Brightness, brightness);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 获取保存的亮度设置
    /// </summary>
    public int GetSavedBrightness()
    {
        return PlayerPrefs.GetInt(DataKeys.Brightness, 50);
    }
    #endregion

    #region 角色位置保存/读取逻辑
    /// <summary> 保存指定场景的角色位置
    /// </summary>
    /// <param name="sceneName">场景名（关联位置所属场景）</param>
    /// <param name="position">角色世界坐标</param>
    public void SavePlayerPosition(string sceneName, Vector3 position)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        // 按(前缀+场景名)存储X/Y/Z坐标（避免不同场景位置冲突）
        string posXKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_X";
        string posYKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_Y";
        string posZKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_Z";

        PlayerPrefs.SetFloat(posXKey, position.x);
        PlayerPrefs.SetFloat(posYKey, position.y);
        PlayerPrefs.SetFloat(posZKey, position.z);
        PlayerPrefs.Save();
    }

    /// <summary> 读取指定场景的角色位置（返回是否存在有效位置）
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="defaultPos">默认位置（无保存时返回）</param>
    /// <returns>有效位置 + 是否存在保存数据</returns>
    public (Vector3 position, bool hasSavedData) GetSavedPlayerPosition(string sceneName, Vector3 defaultPos)
    {
        if (string.IsNullOrEmpty(sceneName))
            return (defaultPos, false);

        string posXKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_X";
        string posYKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_Y";
        string posZKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_Z";

        // 检查是否有该场景的位置数据
        if (!PlayerPrefs.HasKey(posXKey) || !PlayerPrefs.HasKey(posYKey))
            return (defaultPos, false);

        // 读取并返回保存的位置
        Vector3 savedPos = new Vector3(
            PlayerPrefs.GetFloat(posXKey),
            PlayerPrefs.GetFloat(posYKey),
            PlayerPrefs.GetFloat(posZKey)
        );
        return (savedPos, true);
    }

    /// <summary> 删除指定场景的角色位置数据（新游戏/场景重置时用） </summary>
    public void ClearPlayerPosition(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return;

        string posXKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_X";
        string posYKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_Y";
        string posZKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_Z";

        if (PlayerPrefs.HasKey(posXKey)) PlayerPrefs.DeleteKey(posXKey);
        if (PlayerPrefs.HasKey(posYKey)) PlayerPrefs.DeleteKey(posYKey);
        if (PlayerPrefs.HasKey(posZKey)) PlayerPrefs.DeleteKey(posZKey);
    }

    #region 场景Bound进度相关
    /// <summary> 保存当前场景进度 </summary>
    public void SaveCurrentBound(string BoundName)
    {
        PlayerPrefs.SetString(DataKeys.TempSavedBoundName, BoundName);
        PlayerPrefs.Save();
    }

    /// <summary> 获取保存的场景名称 </summary>
    public string GetSavedBoundName()
    {
        return PlayerPrefs.GetString(DataKeys.TempSavedBoundName, string.Empty);
    }
    #endregion

    #endregion

    #region 出战卡牌相关操作
    /// <summary> 从PlayerPrefs加载出战卡牌ID到内存 </summary>
    private void LoadBattleCardIds()
    {
        _battleCardIds.Clear();
        // 读取存储的字符串
        string savedIds = PlayerPrefs.GetString(DataKeys.BattleCardIds, "");
        if (!string.IsNullOrEmpty(savedIds))
        {
            // 分割字符串为ID列表
            string[] idsArray = savedIds.Split(',');
            _battleCardIds.AddRange(idsArray);
        }
    }

    /// <summary> 保存当前出战卡牌ID到PlayerPrefs </summary>
    private void SaveBattleCardIds()
    {
        // 将列表转为逗号分隔的字符串
        string idsStr = string.Join(",", _battleCardIds);
        PlayerPrefs.SetString(DataKeys.BattleCardIds, idsStr);
        PlayerPrefs.Save();
    }

    /// <summary> 获取当前出战卡牌的ID列表（提供给外部调用） </summary>
    public List<string> GetBattleCardINames()
    {
        return new List<string>(_battleCardIds);
    }

    /// <summary> 卡牌到出战列表（返回是否成功） </summary>
    public bool AddBattleCard(string cardName)
    {
        // 校验：ID非空+未在列表中+未达最大数量
        if (string.IsNullOrEmpty(cardName) ||
            _battleCardIds.Contains(cardName) ||
            _battleCardIds.Count >= maxBattleCards)
        {
            Debug.LogWarning($"添加出战卡牌失败：ID为空/已存在/数量已满（当前：{_battleCardIds.Count}/{maxBattleCards}）");
            return false;
        }

        // 添加到内存列表
        _battleCardIds.Add(cardName);
        // 同步保存到PlayerPrefs
        SaveBattleCardIds();
        return true;
    }

    /// <summary> 清空列表 </summary>
    public void ClearBattleCards()
    {
        _battleCardIds.Clear();
        SaveBattleCardIds();
    }

    /// <summary> 检查卡牌是否在出战列表中 </summary>
    public bool IsCardInBattle(string cardName)
    {
        return !string.IsNullOrEmpty(cardName) && _battleCardIds.Contains(cardName);
    }

    #endregion

    #region 精灵亡者首次触发相关
    //保存精灵亡者首次触发状态
    public void SaveElvenUndeadFirstMeeting(bool isFirstMeeting)
    {
        PlayerPrefs.SetInt(DataKeys.ElvenUndeadFirstMeeting, isFirstMeeting ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary> 检查是否首次触发精灵亡者事件 </summary>
    public bool IsElvenUndeadFirstMeeting()
    {
        return PlayerPrefs.GetInt(DataKeys.ElvenUndeadFirstMeeting, 1) == 1;
    }

    #endregion

    #region 重置所有游戏数据
    /// <summary>重置所有游戏数据（新游戏时调用）</summary>
    public void ResetAllGameData()
    {
        foreach (var key in AllDataKeys)
        {
            if (PlayerPrefs.HasKey(key))
                PlayerPrefs.DeleteKey(key);
        }

        //清理所有探索场景的坐标（基于配置列表，而非已加载场景）
        foreach (string sceneName in allExploreSceneNames)
        {
            string posXKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_X";
            string posYKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_Y";
            string posZKey = $"{DataKeys.PlayerPosPrefix}{sceneName}_Z";

            if (PlayerPrefs.HasKey(posXKey)) PlayerPrefs.DeleteKey(posXKey);
            if (PlayerPrefs.HasKey(posYKey)) PlayerPrefs.DeleteKey(posYKey);
            if (PlayerPrefs.HasKey(posZKey)) PlayerPrefs.DeleteKey(posZKey);
        }

        // 重置内存中的过场动画计数
        _currentCinematicCount = 0;
        ClearBattleCards(); // 清空出战列表

        PlayerPrefs.Save();

        // 触发全局数据重置事件（供UI等模块刷新状态）
        EventHandler.CallOnAllDataReset();
    }

    public static class PlayerPrefsUtil
    {
        public static List<string> GetAllKeys()
        {
            List<string> keys = new List<string>();
#if UNITY_EDITOR
            // 编辑器模式仍用反射（方便调试）
            var playerPrefsType = typeof(PlayerPrefs);
            var getKeysMethod = playerPrefsType.GetMethod("GetAllKeys", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (getKeysMethod != null)
            {
                string[] allKeys = (string[])getKeysMethod.Invoke(null, null);
                keys.AddRange(allKeys);
            }
#else
            // 打包后：基于配置列表生成坐标键（无需获取所有PlayerPrefs键）
            foreach (string sceneName in GameDataManager.Instance.allExploreSceneNames)
            {
                keys.Add($"{DataKeys.PlayerPosPrefix}{sceneName}_X");
                keys.Add($"{DataKeys.PlayerPosPrefix}{sceneName}_Y");
                keys.Add($"{DataKeys.PlayerPosPrefix}{sceneName}_Z");
            }
#endif
            return keys;
        }
    }
    #endregion


}
