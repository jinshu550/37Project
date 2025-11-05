using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackUIManager : MonoBehaviour
{
    [Header("基础配置")]
    [SerializeField] private GameObject cardUIPrefab;  // 物品格子预制体
    [SerializeField] private List<Transform> cardContainers = new List<Transform>();  // 物品容器列表
    [SerializeField] private int itemsPerContainer = 3;  // 每个容器显示数量

    [Header("关闭配置")]
    [SerializeField] private Button closeBackpackBtn; // 关闭背包按钮

    [Header("合成配置")]
    [SerializeField] private Button synthesizeButton; // 合成按钮
    [SerializeField] private float shakeMagnitude = 5f; // 抖动幅度
    [SerializeField] private float shakeDuration = 0.3f; // 抖动时长

    [Header("出战配置")]
    [SerializeField] private Button battleButton;  // 出战按钮
    [SerializeField] private BattleFormationManager battleFormationManager; // 关联出战管理器

    [Header("分类按钮")]
    [SerializeField] private Button bodyType1Btn;  // 身材1分类按钮
    [SerializeField] private Button bodyType2Btn;  // 身材2分类按钮
    [SerializeField] private Button bodyType3Btn;  // 身材3分类按钮
    [SerializeField] private Button functionBtn;    // 功能分类按钮

    [Header("描述文本颜色配置")]
    [SerializeField] private Color bodyType1Color = Color.white;
    [SerializeField] private Color bodyType2Color = Color.gray;
    [SerializeField] private Color bodyType3Color = Color.gray;

    // 依赖的子模块
    [SerializeField] private BackpackDisplay displayModule;  // 背包左边UI显示
    [SerializeField] private SelectionManager selectionModule;  //背包物品的选择和显示逻辑
    private BackpackDataService dataService;  //背包的数据服务模块

    public BackpackDataService DataService
    {
        get { return dataService; }
    }

    private bool isBodyCard = true;  // 当前是否为身材卡牌

    private void Awake()
    {
        // new一个数据服务 不需要挂载
        dataService = new BackpackDataService();

        // 绑定出战按钮点击事件（仅转发给专门的管理器）
        if (battleButton != null)
        {
            battleButton.onClick.AddListener(OnBattleButtonClicked);
            battleButton.interactable = false; // 默认禁用
        }

        // 绑定合成按钮点击事件
        if (synthesizeButton != null)
        {
            synthesizeButton.onClick.AddListener(OnSynthesizeButtonClicked);
            synthesizeButton.interactable = false; // 默认禁用
        }

        //绑定关闭按钮
        if (closeBackpackBtn != null)
        {
            closeBackpackBtn.onClick.AddListener(OnCloseBackpackBtnClicked);
        }
    }

    private void OnEnable()
    {
        // 检查 displayModule 是否为空
        if (displayModule == null)
        {
            return; // 避免后续代码崩溃
        }
        // 初始化显示模块（传递必要参数）
        displayModule.Initialize(cardUIPrefab, cardContainers, itemsPerContainer, selectionModule);

        StartCoroutine(DelayShowCategory(CategoryType.BodyType1));//延迟一帧显示当前的按钮
        // 订阅选择变化事件，更新合成按钮状态
        EventHandler.OnSelectionChanged += UpdateSynthesizeButtonState;
        EventHandler.OnSynthesisFailed += OnSynthesisFailed;
    }

    private void OnDisable()
    {
        selectionModule.ClearAllSelections();  //关闭时清除选中状态
        EventHandler.OnSelectionChanged -= UpdateSynthesizeButtonState;

        EventHandler.CallOnSelectionCleared();// 触发选择清除事件，销毁未收入的合成预设体
        EventHandler.OnSynthesisFailed -= OnSynthesisFailed;
    }

    private void Start()
    {
        // 绑定分类按钮事件
        functionBtn.onClick.AddListener(() => ShowCategory(CategoryType.BackFunction));
        bodyType1Btn.onClick.AddListener(() => ShowCategory(CategoryType.BodyType1));
        bodyType2Btn.onClick.AddListener(() => ShowCategory(CategoryType.BodyType2));
        bodyType3Btn.onClick.AddListener(() => ShowCategory(CategoryType.BodyType3));

    }

    void Update()
    {
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.B))
        {
            OnCloseBackpackBtnClicked();
        }
    }

    /// <summary>延迟一帧触发，确保事件订阅完成</summary>
    private IEnumerator DelayShowCategory(CategoryType category)
    {
        yield return null; // 等待下一帧（让BackpackDisplay完成OnEnable和事件订阅）
        ShowCategory(category);
    }

    /// <summary> 背包UI显示指定分类展示的卡牌
    /// </summary>
    /// <param name="category"></param>
    private void ShowCategory(CategoryType category)
    {
        isBodyCard = category != CategoryType.BackFunction;

        // 检查 selectionModule 是否为空
        if (selectionModule == null)
        {
            return;
        }
        // 通知选择模块切换模式
        selectionModule.SetSelectionMode(isBodyCard);
        // 清除之前选中的卡牌
        selectionModule.ClearAllSelections();
        EventHandler.CallOnSelectionCleared();

        // 切换分类时更新合成，出战按钮状态
        if (battleButton != null)
        {
            battleButton.interactable = !isBodyCard;  // 只有非身材卡牌可出战
        }

        if (synthesizeButton != null)
        {
            synthesizeButton.interactable = false;  //仅功能分类（非身材卡牌）可合成
        }

        // 根据分类设置描述文本颜色
        Color targetColor = category switch
        {
            CategoryType.BodyType1 => bodyType1Color,
            CategoryType.BodyType2 => bodyType2Color,
            CategoryType.BodyType3 => bodyType3Color,
            _ => bodyType1Color
        };

        //触发颜色变更事件
        EventHandler.CallOnDescriptionColorChanged(targetColor);
        // 获取数据并刷新UI
        List<CardBasicInformation> data = dataService.GetCategoryData(category);

        //检查出战状态
        if (category == CategoryType.BackFunction)
        {
            //如果是点击功能按钮就刷新UI恢复出战状态
            if (battleFormationManager != null)
            {
                StartCoroutine(RestoreBattleStateAfterUI());
            }
        }
        EventHandler.CallUpdateInventoryUI(data); // 触发事件

    }

    #region 合成按钮相关
    /// <summary>根据选择状态更新合成按钮可用性（仅多选2-3张卡牌时启用）</summary>
    private void UpdateSynthesizeButtonState(bool isSingleMode, BackCardUI singleCard, List<BackCardUI> multiCards)
    {
        if (synthesizeButton == null || isSingleMode || isBodyCard)
        {
            synthesizeButton.interactable = false;
            return;
        }

        // 仅当选中2-3张卡牌时启用合成按钮
        bool canRequestSynthesis = multiCards != null && multiCards.Count >= 2 && multiCards.Count <= 3;
        synthesizeButton.interactable = canRequestSynthesis;
    }

    /// <summary>合成按钮点击：仅触发合成请求事件，不处理具体逻辑</summary>
    private void OnSynthesizeButtonClicked()
    {
        if (selectionModule == null) return;

        // 获取当前选中的卡牌列表
        List<BackCardUI> selectedCards = selectionModule.GetMultiSelectedCards();
        if (selectedCards == null || selectedCards.Count < 2) return;

        // 触发合成请求（交给CardSynthesisManager处理）
        EventHandler.CallOnSynthesisRequested(selectedCards);

        //检查出战状态
        //如果是点击功按钮就刷新UI恢复出战状态
        if (battleFormationManager != null)
        {
            StartCoroutine(RestoreBattleStateAfterUI());
        }

    }

    /// <summary>合成失败时触发：执行按钮抖动</summary>
    private void OnSynthesisFailed()
    {
        if (synthesizeButton == null) return;

        // 启动抖动协程
        StartCoroutine(ShakeButtonCoroutine(synthesizeButton.transform));
    }

    /// <summary>按钮抖动协程（控制位置偏移实现抖动）</summary>
    private IEnumerator ShakeButtonCoroutine(Transform btnTransform)
    {
        Vector3 originalPos = btnTransform.localPosition; // 记录初始位置（用localPosition避免父物体影响）
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // 随机偏移X/Y轴（抖动核心逻辑）
            float randomX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float randomY = Random.Range(-shakeMagnitude, shakeMagnitude);
            btnTransform.localPosition = new Vector3(originalPos.x + randomX, originalPos.y + randomY, originalPos.z);

            elapsedTime += Time.deltaTime;
            yield return null; // 等待一帧
        }

        // 抖动结束后复位到初始位置
        btnTransform.localPosition = originalPos;
    }

    #endregion

    #region 出战状态恢复相关

    /// <summary>
    /// 转发点击事件给专门的出战管理器
    /// </summary>
    private void OnBattleButtonClicked()
    {
        battleFormationManager.OnBattleButtonClick();
    }

    private IEnumerator RestoreBattleStateAfterUI()
    {
        yield return null; // 等待一帧，确保卡牌UI已生成
        battleFormationManager.RestoreBattleStateFromDataManager();
    }

    #endregion
    /// <summary> 关闭背包面板：隐藏面板并触发清理逻辑 </summary>
    public void OnCloseBackpackBtnClicked()
    {
        // 隐藏背包面板（禁用面板时，会自动执行OnDisable逻辑）
        gameObject.SetActive(false);
        //隐藏背包-->解锁输入
        EventHandler.CallOnDialogueStateChanged(false);
    }

    public enum CategoryType
    {
        BackFunction,
        BodyType1,
        BodyType2,
        BodyType3
    }
}