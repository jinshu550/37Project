using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackToggleBtn : MonoBehaviour
{
    [SerializeField] private Button backpackBtn; // 背包按钮
    [SerializeField] private BackpackUIManager backpackPanelManager; // 背包面板上的BackpackUIManager组件
    private bool isEnabled = true;// 输入是否启用

    private void Awake()
    {
        if (backpackBtn == null)
        {
            backpackBtn = GetComponent<Button>();
            return;
        }

        // 绑定按钮点击事件：点击显示背包面板
        backpackBtn.onClick.AddListener(OnBackpackBtnClicked);

        //场景加载时强制隐藏背包面板
        if (backpackPanelManager != null && backpackPanelManager.gameObject != null && backpackPanelManager.gameObject.activeSelf)
            backpackPanelManager.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        // 监听对话事件：控制输入开关
        EventHandler.OnDialogueStateChanged += OnDialogueStateChanged;

        //场景加载时强制隐藏背包面板
        if (backpackPanelManager != null && backpackPanelManager.gameObject != null && backpackPanelManager.gameObject.activeSelf)
            backpackPanelManager.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // 移除监听
        EventHandler.OnDialogueStateChanged -= OnDialogueStateChanged;
    }

    void Update()
    {
        if (isEnabled)
        {
            if (Input.GetKeyDown(KeyCode.B) && backpackPanelManager != null)
            {
                if (!backpackPanelManager.gameObject.activeSelf)
                {

                    ToggleBackpackPanel();
                }
            }
        }
    }

    /// <summary>切换背包面板状态（打开/关闭）</summary>
    private void ToggleBackpackPanel()
    {
        if (backpackPanelManager == null) return;

        bool isActive = backpackPanelManager.gameObject.activeSelf;
        // 切换状态：显示→隐藏，隐藏→显示
        backpackPanelManager.gameObject.SetActive(!isActive);

        // 同步输入锁定状态：打开时锁定，关闭时解锁
        EventHandler.CallOnDialogueStateChanged(!isActive);
    }

    /// <summary>背包按钮点击：显示面板并触发初始默认显示</summary>
    private void OnBackpackBtnClicked()
    {
        if (backpackPanelManager == null)
        {
            Debug.LogError("BackpackToggleBtn: 未赋值BackpackUIManager！请拖入背包面板的组件");
            return;
        }

        // 显示背包面板（激活面板时，会自动执行BackpackUIManager的OnEnable逻辑）
        backpackPanelManager.gameObject.SetActive(true);
        //显示背包-->锁定输入
        EventHandler.CallOnDialogueStateChanged(true);
    }

    private void OnDialogueStateChanged(bool isDialogueActive)
    {
        isEnabled = !isDialogueActive;
    }

}
