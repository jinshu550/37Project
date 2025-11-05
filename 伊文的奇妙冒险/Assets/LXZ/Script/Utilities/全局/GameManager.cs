using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : ExploreSingleton<GameManager>
{
    [Header("玩家预制体")]
    public GameObject playerPrefab; // 角色预制体
    private GameObject currentPlayer; //当前的角色

    [Header("全局UI面板")]
    public GameObject inventoryPanel; // 拖入你的UI预制体Canvas
    private Button backBtn;        // 需控制显隐的背包按钮
    private Button mapBtn;        // 需控制显隐的地图按钮
    private Button settingBtn;    // 设置按钮
    private GameObject currentInventoryPanel; //当前UI面板

    [Header("音量亮度配置")]
    [SerializeField] private string bgmMixerParam = "BGM";  // BGM混合器参数名
    [SerializeField] private string sfxMixerParam = "SFX";  // SFX混合器参数名
    public static readonly int DefaultBgmVolume = 70;     //初始背景音乐
    public static readonly int DefaultSfxVolume = 80;    //初始音效
    public static readonly int DefaultBrightness = 40;    //初始光照
    public static readonly float DefaultMinLightIntensity = 0.8f; // 全局Light2D最小强度
    public static readonly float DefaultMaxLightIntensity = 1.3f;  // 全局Light2D最大强度
    public static readonly string GlobalLight2DTAG = "GlobalLight2D"; // 全局Light2D标签（统一）
    public Light2D currentSceneLight2D;

    [Header("场景前缀配置")]
    public string exploreScenePrefix = "Explore_"; //探索场景的名称前缀, 通过这个来判断是否保留角色和 UI
    public string battleScenePrefix = "Fight_";   // 战斗场景前缀

    [Header("设置按钮位置配置")]
    [SerializeField] private Vector2 exploreSettingBtnPos = new Vector2(50, 38); // 探索场景中设置按钮的位置
    [SerializeField] private Vector2 battleSettingBtnPos = new Vector2(1670, 38);   // 战斗场景中设置按钮的位置

    private bool isMainMenu;

    protected override void Awake()
    {
        base.Awake(); // 先执行基类的单例逻辑

        EventHandler.OnBackpackUnlocked += OnBackpackUnlocked;
        EventHandler.OnAudioManagerInited += OnAudioManagerInited;
        EventHandler.OnAllDataReset += OnDataReset; // 监听数据重置

        Game.GetInstance();
    }
    private void Start()
    {
        if (inventoryPanel != null && currentInventoryPanel == null)
        {
            GameObject existingPanel = GameObject.FindGameObjectWithTag("GlobalInventory");
            if (existingPanel != null)
            {
                currentInventoryPanel = existingPanel; // 复用已有面板
            }
            else
            {
                // 没有面板才新建
                currentInventoryPanel = Instantiate(inventoryPanel);
                currentInventoryPanel.tag = "GlobalInventory"; // 标记唯一标签
                DontDestroyOnLoad(currentInventoryPanel);
            }

            FindAllFunctionButtons();

            isMainMenu = SceneManager.GetActiveScene().name == "MainMenu";
            currentInventoryPanel.SetActive(!isMainMenu); // 初始隐藏
            
        }

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 注册场景加载事件
        GetCurrentSceneLight2D();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 取消注册
    }

    private void OnDestroy()
    {
        EventHandler.OnBackpackUnlocked -= OnBackpackUnlocked;
        EventHandler.OnAudioManagerInited -= OnAudioManagerInited;
        EventHandler.OnAllDataReset -= OnDataReset;
    }

    #region UI面板的相关设置
    /// <summary>  查找所有功能按钮 </summary>
    private void FindAllFunctionButtons()
    {
        Transform backBtnTrans = currentInventoryPanel.transform.Find("BackBtn");
        Transform mapBtnTrans = currentInventoryPanel.transform.Find("mapBtn");
        Transform settingBtnTrans = currentInventoryPanel.transform.Find("SettingBtn"); // 假设设置按钮名为"SettingBtn"

        backBtn = backBtnTrans?.GetComponent<Button>();
        mapBtn = mapBtnTrans?.GetComponent<Button>();
        settingBtn = settingBtnTrans?.GetComponent<Button>();

        // 空引用警告
        if (settingBtn == null)
            Debug.LogWarning("未找到设置按钮，请检查面板中是否存在名为'SettingBtn'的按钮");
    }

    /// <summary> 背包解锁回调 </summary>
    private void OnBackpackUnlocked()
    {
        GameDataManager.Instance.UnlockBackpack(); // 委托给数据管理器
        UpdateBackpackButtonState();
    }

    /// <summary> 更新背包按钮状态 </summary>
    private void UpdateBackpackButtonState()
    {
        if (backBtn == null) return;
        bool isExploreScene = SceneManager.GetActiveScene().name.StartsWith(exploreScenePrefix);
        backBtn.gameObject.SetActive(GameDataManager.Instance.HasBackpackUnlocked() && isExploreScene);
    }

    /// <summary>加载并应用所有全局设置（音量、亮度）</summary>
    private void OnAudioManagerInited()
    {
        var (bgmVol, sfxVol) = GameDataManager.Instance.GetSavedVolumes();

        // ---------- 加载BGM音量 ----------
        EventHandler.CallOnGroupVolumeChanged(bgmMixerParam, bgmVol);
        //EventHandler.CallOnGroupVolumeChanged("BGM", bgmVol); // 事件通知（若其他系统依赖）

        // ---------- 加载SFX音量 ----------
        EventHandler.CallOnGroupVolumeChanged(sfxMixerParam, sfxVol);
        //ventHandler.CallOnGroupVolumeChanged("SFX", sfxVol);

        // ---------- 加载全局亮度 ----------
        SyncGlobalLight2DToSavedBrightness();
    }

    /// <summary> 触发地图按钮的点击效果 </summary>
    public void OpenMap()
    {
        if (mapBtn != null)
        {
            // 直接触发按钮的点击事件  
            mapBtn.onClick.Invoke();
        }
        else
        {
            Debug.LogError("mapBtn未找到，无法触发地图按钮效果！请检查背包面板中是否有mapBtn");
        }
    }

    #endregion

    #region  场景加载相关设置
    // 场景加载完成后调用
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isMainMenu = SceneManager.GetActiveScene().name == "MainMenu";

        //检查场景名称是否包含前缀
        bool isExploreScene = scene.name.StartsWith(exploreScenePrefix);
        bool isBattleScene = scene.name.StartsWith(battleScenePrefix); //战斗场景判断

        SaveLastScenePlayerPosition(scene);//在新场景加载时，先保存上一个场景的位置

        if (currentInventoryPanel != null)
            currentInventoryPanel.SetActive(!isMainMenu); //初始隐藏
        if (isMainMenu) return;

        if (isExploreScene)
        {
            // 探索场景：生成角色
            SpawnPlayer();
            GameDataManager.Instance.SaveCurrentScene(scene.name);// 通过数据管理器保存场景

            // 显示背包UI（如果需要）
            UpdateBackpackButtonState();
            mapBtn?.gameObject.SetActive(true);

        }
        else
        {
            // 非探索场景：销毁角色（如果存在）
            if (currentPlayer != null)
                Destroy(currentPlayer);
            // 隐藏背包UI
            backBtn.gameObject.SetActive(false);
            mapBtn.gameObject.SetActive(false);
        }

        //根据场景类型更新设置按钮位置
        UpdateSettingButtonPosition(isExploreScene, isBattleScene);

    }

    /// <summary>根据场景类型更新设置按钮位置</summary>
    private void UpdateSettingButtonPosition(bool isExplore, bool isBattle)
    {
        if (settingBtn == null) return;

        // 获取设置按钮的RectTransform（用于调整UI位置）
        RectTransform settingRect = settingBtn.GetComponent<RectTransform>();
        if (settingRect == null)
        {
            Debug.LogWarning("设置按钮缺少RectTransform组件");
            return;
        }

        // 根据场景类型设置位置
        if (isExplore)
        {
            settingRect.anchoredPosition = exploreSettingBtnPos;
            settingBtn.gameObject.SetActive(true); // 探索场景显示设置按钮
        }
        else if (isBattle)
        {
            settingRect.anchoredPosition = battleSettingBtnPos;
            settingBtn.gameObject.SetActive(true); // 战斗场景显示设置按钮
        }
        else
        {
            settingBtn.gameObject.SetActive(false); // 其他场景隐藏设置按钮
        }
    }

    /// <summary> 生成玩家到场景的"PlayerSpawn"位置 </summary>
    private void SpawnPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer); // 销毁上一场景的玩家
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        Vector3 spawnPos = Vector3.zero;

        //  尝试读取当前场景的保存位置
        var (savedPos, hasSavedData) = GameDataManager.Instance.GetSavedPlayerPosition(
            currentSceneName,
            spawnPos // 默认值
        );

        if (hasSavedData)
        {
            // 有保存位置：用保存的坐标
            spawnPos = savedPos;

            //加载结束后清除位置，避免下次进场景时误用
            GameDataManager.Instance.ClearPlayerPosition(currentSceneName);
            LoadTempBoundAndPlayerPos(); //加载临时保存的bound
        }
        else
        {
            // 无保存位置：找场景中的PlayerSpawn
            GameObject spawnPointObj = GameObject.Find("PlayerSpawn");

            //清除静态变量，避免下次误用
            if (GameDataManager.Instance.GetSavedBoundName() != "")
            {
                GameDataManager.Instance.SaveCurrentBound("");
            }

            if (spawnPointObj != null)
            {
                spawnPos = spawnPointObj.transform.position;
            }
            else
            {
                Debug.LogError($"场景[{currentSceneName}]中无PlayerSpawn，使用世界原点生成！");
            }
        }

        //生成角色
        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }

    /// <summary> 保存上一场景角色的位置 </summary>
    private void SaveLastScenePlayerPosition(Scene scene)
    {
        // 仅处理探索场景
        if (!scene.name.StartsWith(exploreScenePrefix))
            return;

        // 保存上一场景的玩家位置
        if (currentPlayer != null)
        {
            GameDataManager.Instance.SavePlayerPosition(scene.name, currentPlayer.transform.position);
        }
    }

    /// <summary> 供摄像机获取当前生成的角色
    /// </summary>
    /// <returns></returns>
    public Transform GetCurrentPlayerTransform()
    {
        if (currentPlayer != null)
            return currentPlayer.transform; // 返回角色的Transform（摄像机需要跟随这个）
        return null;
    }

    private void LoadTempBoundAndPlayerPos()
    {
        // 1. 查找CameraFollow和保存的bound
        CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
        if (cameraFollow == null || string.IsNullOrEmpty(GameDataManager.Instance.GetSavedBoundName()))
        {
            return;
        }

        // 按保存的bound名称查找场景2中的第2个bound
        Collider2D[] allBounds = FindObjectsOfType<Collider2D>();
        Collider2D targetBound = null;
        foreach (var bound in allBounds)
        {
            if (bound.name == GameDataManager.Instance.GetSavedBoundName())
            {
                targetBound = bound;
                break;
            }
        }

        // 赋值bound到CameraFollow
        if (targetBound != null)
        {
            cameraFollow.boundary = targetBound;
            AudioManager.instance.PlayBGM("BGM_Scene2_2");
        }
        else
        {
            Debug.LogWarning($"场景2未找到保存的bound：{GameDataManager.Instance.GetSavedBoundName()}，使用默认bound");
        }

        // 加载完成后，立即清除静态数据
        GameDataManager.Instance.SaveCurrentBound("");
    }

    #endregion


    /// <summary>数据重置后刷新状态 </summary>
    private void OnDataReset()
    {
        UpdateBackpackButtonState(); //重置背包按钮状态

        // 新游戏重置时，销毁当前玩家并清理当前场景位置
        if (currentPlayer != null)
            Destroy(currentPlayer);

        string currentSceneName = SceneManager.GetActiveScene().name;
        GameDataManager.Instance.ClearPlayerPosition(currentSceneName);

        GameDataManager.instance.SaveVolumeSettings(DefaultBgmVolume, DefaultSfxVolume);
        GameDataManager.Instance.SaveBrightness(DefaultBrightness);
    }
    /// <summary> 同步光照 </summary>
    private void SyncGlobalLight2DToSavedBrightness()
    {
        int savedBrightness = GameDataManager.Instance.GetSavedBrightness();

        if (currentSceneLight2D == null)
        {

            currentSceneLight2D = FindGlobalLight2D();
            return;
        }

        // 计算保存的亮度对应的Light2D强度（与SettingPanelManager逻辑一致）
        float targetIntensity = DefaultMinLightIntensity + savedBrightness / 100f * (DefaultMaxLightIntensity - DefaultMinLightIntensity);
        currentSceneLight2D.intensity = Mathf.Clamp(targetIntensity, DefaultMinLightIntensity, DefaultMaxLightIntensity);

    }

    /// <summary>构建的全局Light2D</summary>
    private void GetCurrentSceneLight2D()
    {
        GameObject lightObj = new GameObject("Global Light 2D ");

        currentSceneLight2D = lightObj.AddComponent<Light2D>();
        lightObj.tag = GlobalLight2DTAG;
        currentSceneLight2D.lightType = Light2D.LightType.Global;

        DontDestroyOnLoad(lightObj); //保留物体跨场景

        //如果还是空就直接返回
        if (currentSceneLight2D == null)
        {
            Debug.LogError($"当前场景「{SceneManager.GetActiveScene().name}」未找到标签为「{GlobalLight2DTAG}」的Light2D！请给全局光照设置正确标签");
            return;
        }
    }

    /// <summary> 查找当前场景的全局Light2D
    /// </summary>
    /// <returns></returns>
    public Light2D FindGlobalLight2D()
    {
        Light2D[] allLight2D = FindObjectsOfType<Light2D>(includeInactive: false);
        foreach (var light2D in allLight2D)
        {
            if (light2D.CompareTag(GlobalLight2DTAG))
                return light2D;
        }
        Debug.LogWarning($"当前场景未找到标签为「{GlobalLight2DTAG}」的全局Light2D！");
        return null;
    }

}