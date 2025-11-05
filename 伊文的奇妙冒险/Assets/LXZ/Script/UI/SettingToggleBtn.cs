using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingToggleBtn : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel; // 控制设置面板本体
    [Header("按钮")]
    [SerializeField] private Button closeBtn; // 面板内的关闭按钮
    [SerializeField] private Button exitBtn; // 面板内的退出按钮

    [SerializeField] private Button settingBtn;       // 引用自身按钮
    private bool isEnabled = true; // 输入是否启用

    private void Awake()
    {
        // 绑定按钮事件
        closeBtn?.onClick.AddListener(OnCloseBtnClicked);
        exitBtn?.onClick.AddListener(OnExitBtnClicked);

        if (settingPanel != null && settingPanel.activeSelf)
            settingPanel.SetActive(false);
        // 绑定自身按钮点击事件（打开/关闭面板）
        if (settingBtn == null)
            settingBtn = GetComponent<Button>();
        if (settingBtn != null)
        {
            settingBtn.onClick.AddListener(TogglePanel);
        }
    }

    private void OnEnable()
    {
        // 监听对话事件：对话时禁用面板操作
        EventHandler.OnDialogueStateChanged += OnDialogueStateChanged;
        //面板激活时再次确认隐藏
        if (settingPanel != null && settingPanel.activeSelf)
            settingPanel.SetActive(false);
    }

    private void OnDisable()
    {
        EventHandler.OnDialogueStateChanged -= OnDialogueStateChanged;
    }

    private void Update()
    {

        // ESC键控制：仅输入启用时，切换面板显示 ，锁输入后只能控制对应打开的面板
        if (Input.GetKeyDown(KeyCode.Escape) && isEnabled && !settingPanel.activeSelf)
        {
            //打开面板
            TogglePanel();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && settingPanel.activeSelf)
        {
            //关闭面板
            TogglePanel();
        }
    }

    /// <summary> 切换面板显隐 + 同步输入锁定 </summary>
    public void TogglePanel()
    {
        if (settingPanel == null)
        {
            Debug.LogError("SettingToggleBtn: 未赋值设置面板！");
            return;
        }

        bool isPanelActive = settingPanel.activeSelf;
        // 切换面板状态
        settingPanel.SetActive(!isPanelActive);

        // 同步输入锁定：面板打开→锁定其他操作（如移动、背包）；关闭→解锁
        EventHandler.CallOnDialogueStateChanged(!isPanelActive);
    }

    #region 按钮事件
    private void OnCloseBtnClicked()
    {
        settingPanel.SetActive(false);
        EventHandler.CallOnDialogueStateChanged(false);//返回解除输入
    }

    private void OnExitBtnClicked()
    {
        Loader.SetTargetScene("MainMenu"); //返回主界面
        ActionSystem.ClearAllSubscriptions();
        StaticUtility.ClearPartData();
        EventHandler.CallOnDialogueStateChanged(false);
    }
    #endregion

    private void OnDialogueStateChanged(bool isDialogueActive)
    {
        isEnabled = !isDialogueActive;
    }
}
