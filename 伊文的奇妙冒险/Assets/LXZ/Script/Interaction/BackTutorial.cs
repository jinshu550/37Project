using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class BackTutorial : MonoBehaviour
{
    [Header("提示面板")]
    [SerializeField] private GameObject promptPanel;

    [Header("收集后的剧情提示教程等")]
    [SerializeField] private NPCDialogueSystem npcDialogueSystem; // 关联的对话组件
    [SerializeField] private GameObject confirmPopup;   // 弹窗面板本体（初始隐藏）
    [SerializeField] private TMP_Text popupTitle; // 弹窗标题文本
    [SerializeField] private TMP_Text popupContent; // 弹窗内容文本
    [SerializeField] private Button confirmBtn;         // 弹窗确认按钮
    [SerializeField] private string popupTitleText = "获得背包";         // 弹窗标题文本
    [SerializeField] private string popupContentText = "现在可以打开背包啦";         // 弹窗内容文本

    [Header("点击设置")]
    [SerializeField] private string targetTag = "Collect";//检测标签避免点击其他地方
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Camera mainCamera;

    private bool isPlayerInRange = false;  // 是否在检测范围内
    private bool isTutorialCompleted = false; // 教程是否已完成
    private bool isPopupShowing = false;  // 弹窗是否正在显示
    private bool isEnabled = true;// 输入是否启用


    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        //初始化弹窗：默认隐藏，绑定确认按钮事件
        InitConfirmPopup();
        //初始物体
        gameObject.SetActive(true);

        //检查背包是否解锁，如果解锁直接跳过需要解锁功能
        CheckBackpackUnlockedState();
    }

    private void OnEnable()
    {
        // 监听对话事件
        EventHandler.OnDialogueStateChanged += OnDialogueStateChanged;
    }

    private void OnDisable()
    {
        // 移除监听
        EventHandler.OnDialogueStateChanged -= OnDialogueStateChanged;
    }

    void Update()
    {
        if (!isEnabled)
            return;

        // 教程开启后 显示框还没显示 在开启输入后 就会显示获取框
        if (isTutorialCompleted && !isPopupShowing)
        {
            ShowCollectConfirmPopup();
            return;
        }

        // 非探索场景不响应拾取
        if (!SceneManager.GetActiveScene().name.StartsWith(GameManager.Instance.exploreScenePrefix))
            return;

        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0) && isPlayerInRange && mainCamera != null)
        {
            // 忽略UI点击
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            // 从相机发射射线到鼠标位置
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            // 转换为2D射线检测（检测2D碰撞体）
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, targetLayer);

            // 如果射线命中了物体
            if (hit)
            {
                // 检查是否是对应标签的物品
                if (hit.collider.CompareTag(targetTag))
                {
                    StartInteraction();
                }
            }
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartInteraction();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTutorialCompleted) // 教程开始 不再显示提示面板
            return;

        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (promptPanel != null)
                promptPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isTutorialCompleted) //教程开始 不再显示提示面板
            return;

        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptPanel != null)
                promptPanel.SetActive(false);
        }
    }

    public void StartInteraction()
    {
        //如果教程已完成 或 正在播放教程 就退出
        if (isTutorialCompleted) return;

        isTutorialCompleted = true; //开始教程

        //隐藏面板
        promptPanel.SetActive(false);

        //若未赋值对话组件，直接加载场景
        if (npcDialogueSystem == null)
        {
            Debug.LogError($"[{gameObject.name}] 未配置！无法触发过渡剧情", this);
            ShowCollectConfirmPopup(); //如果没赋值就直接出现获取框
            return;
        }

        npcDialogueSystem.StartDialogueByEvent();//触发NPC对话

    }

    /// <summary> 显示收集后的确认弹窗 </summary>
    private void ShowCollectConfirmPopup()
    {
        if (confirmPopup == null)
            return;

        isPopupShowing = true; //获取框开始显示

        // 只有“对话未运行”时,显示弹窗，标记弹窗状态
        if (!confirmPopup.activeSelf)
        {
            confirmPopup.SetActive(true);
            EventHandler.CallOnDialogueStateChanged(true);
        }

    }

    /// <summary> 获取弹窗的确认按钮点击回调 </summary>
    private void OnConfirmBtnClicked()
    {
        // 隐藏弹窗，重置弹窗状态
        if (confirmPopup != null)
            confirmPopup.SetActive(false);

        //isPopupShowing = false;
        EventHandler.CallOnDialogueStateChanged(false);

        // 执行背包解锁（点击确认后才解锁）
        CompleteBackpackTutorialStep();
    }

    /// <summary>  执行背包解锁的通知 </summary>
    public void CompleteBackpackTutorialStep()
    {
        isTutorialCompleted = false;// 标记教程完成

        // 触发背包解锁事件（通知数据管理器和UI）
        EventHandler.CallOnBackpackUnlocked();

        // 销毁自己
        // Destroy(gameObject);
        gameObject.SetActive(false);

        //教程完成后禁用自身碰撞体，防止再次触发
        // Collider2D collider = GetComponent<Collider2D>();
        // if (collider != null)
        //     collider.enabled = false;

    }

    #region 初始化设置
    /// <summary> 检查背包是否已解锁，若已解锁则标记教程完成 </summary>
    private void CheckBackpackUnlockedState()
    {
        // 避免 GameDataManager 实例为空导致报错
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("BackTutorial: 未找到GameDataManager实例，默认按未解锁处理");
            return;
        }

        // 若背包已解锁，直接标记教程完成
        if (GameDataManager.Instance.HasBackpackUnlocked())
        {
            CompleteBackpackTutorialStep();
        }
    }

    /// <summary> 初始化确认弹窗  </summary>
    private void InitConfirmPopup()
    {
        // 初始隐藏弹窗
        if (confirmPopup != null)
            confirmPopup.SetActive(false);

        // 绑定确认按钮点击事件
        if (confirmBtn != null)
            confirmBtn.onClick.AddListener(OnConfirmBtnClicked);
        else
            Debug.LogError("BackTutorial: 确认弹窗的「confirmBtn」未赋值！");

        // 检查弹窗文本组件（可选：设置默认文本）
        if (popupTitle != null)
            popupTitle.text = popupTitleText;
        if (popupContent != null)
            popupContent.text = popupContentText;
    }
    #endregion

    private void OnDialogueStateChanged(bool isDialogueActive)
    {
        if (isDialogueActive)
        {
            // 禁用输入
            isEnabled = false;
        }
        else
        {
            // 启用输入
            isEnabled = true;
        }


    }
}