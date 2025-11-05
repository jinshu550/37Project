using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("按钮引用")]
    [SerializeField] private Button GameBtn;  // 开始游戏按钮
    [SerializeField] private Button newGameBtn;   // 新游戏按钮
    [SerializeField] private Button craftingBtn;   // 制作按钮
    [SerializeField] private Button exitGameBtn;   // 退出游戏按钮

    [Header("制作面板")]
    [SerializeField] private GameObject craftingPanel; // 制作面板UI

    [Header("场景配置")]
    [SerializeField] private string firstLevelName = "Explore_001Scene"; // 首个游戏场景名称


    private void Awake()
    {
        // 绑定按钮事件（检查空引用，避免报错）
        if (GameBtn != null)
            GameBtn.onClick.AddListener(OnStartGameClicked);


        if (newGameBtn != null)
            newGameBtn.onClick.AddListener(OnNewGameClick);

        if (craftingBtn != null)
            craftingBtn.onClick.AddListener(OnCraftingClicked);

        if (exitGameBtn != null)
            exitGameBtn.onClick.AddListener(OnExitGameClicked);

        // 初始化制作面板状态（默认隐藏）
        if (craftingPanel != null)
            craftingPanel.SetActive(false);
    }

    private void Start()
    {
        // 初始化：判断是否有保存的游戏数据，控制“继续游戏”按钮是否可用
        UpdateContinueBtnState();
    }

    private void UpdateContinueBtnState()
    {
        if (GameBtn == null) return;

        // 判断是否有保存的游戏数据
        bool hasSavedData = !string.IsNullOrEmpty(GameDataManager.Instance.GetSavedSceneName()) || GameDataManager.Instance.HasBackpackUnlocked() || GameDataManager.Instance.IsSceneUnlocked(2);

        // 若第一次打开游戏，不显示新游戏按钮 并只有开始游戏
        newGameBtn.gameObject.SetActive(hasSavedData);
        // 修改按钮文本（比如“继续游戏”）
        TMP_Text continueBtnText = GameBtn.GetComponentInChildren<TMP_Text>();
        if (continueBtnText != null)
        {
            continueBtnText.text = hasSavedData ? "继续游戏" : "开始游戏";
            if(hasSavedData)
            {
                if(GameDataManager.Instance.GetSavedBoundName()!=null)
                {
                    
                }
            }
        }
        GameBtn.interactable = true;
    }

    #region 按钮点击事件
    /// <summary> 开启新游戏 </summary>
    private void OnNewGameClick()
    {
        // 调用数据管理器重置所有数据
        GameDataManager.Instance.ResetAllGameData();

        // 2. 加载第一个探索场景（从GameManager获取场景名，避免硬编码）
        if (!string.IsNullOrEmpty(firstLevelName))
        {
            // 主界面淡出后加载加载场景
            Loader.SetTargetScene(firstLevelName);
        }
        else
        {
            Debug.LogError("无法加载新游戏。");
        }

    }

    /// <summary>开始游戏按钮点击：加载首个游戏场景</summary>
    private void OnStartGameClicked()
    {
        string targetScene = GameDataManager.Instance.GetSavedSceneName(); //加载存储的游戏场景

        bool isSceneValid = !string.IsNullOrEmpty(targetScene) && targetScene.StartsWith(GameManager.Instance?.exploreScenePrefix ?? "Explore_"); //判断名称不为空 并是探索场景

        if (!isSceneValid) //如果没有
        {
            targetScene = firstLevelName;

        }

        Loader.SetTargetScene(targetScene);
    }


    /// <summary>制作按钮点击：显示/隐藏制作面板</summary>
    private void OnCraftingClicked()
    {
        if (craftingPanel != null)
        {
            // 切换面板显示状态（显示↔隐藏）
            craftingPanel.SetActive(!craftingPanel.activeSelf);
        }
        else
        {
            Debug.LogError("请在Inspector中赋值制作面板！");
        }
    }

    /// <summary>退出游戏按钮点击：退出应用</summary>
    private void OnExitGameClicked()
    {
#if UNITY_EDITOR
        // 编辑器模式下停止运行
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 打包后退出应用
        Application.Quit();
#endif
    }
    #endregion



}
