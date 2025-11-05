using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapPanelManager : MonoBehaviour
{
    [Header("面板引用")]
    [SerializeField] private GameObject mapPanel; // 地图面板本身
    [SerializeField] private GameObject scene2LockIcon; // 场景二锁定图标

    [Header("按钮引用")]
    [SerializeField] private Button closeButton;   // 关闭按钮
    [SerializeField] private Button scene1Button;  // 场景一跳转按钮
    [SerializeField] private Button scene2Button;  // 场景二跳转按钮

    [Header("场景名称配置")]
    [SerializeField] private string scene1Name = "Explore_001Scene"; // 场景一名称
    [SerializeField] private string scene2Name = "Explore_002Scene"; // 场景二名称

    

    private bool isEnabled = true;// 输入是否启用

    private void Awake()
    {
        // 绑定按钮事件
        if (closeButton != null)
            closeButton.onClick.AddListener(ToggleMapPanel);

        if (scene1Button != null)
            scene1Button.onClick.AddListener(JumpToScene1);

        if (scene2Button != null)
            scene2Button.onClick.AddListener(JumpToScene2);

        //确认隐藏
        if (mapPanel != null && mapPanel.activeSelf)
            mapPanel.SetActive(false);

        //如果场景二解锁就隐藏锁定图标 ，否则显示
        if (GameDataManager.Instance.IsSceneUnlocked(2))
        {
            scene2LockIcon.SetActive(false);
        }
        else
        {
            scene2LockIcon.SetActive(true);
        }
    }

    private void OnEnable()
    {
        // 监听对话事件：控制移动开关
        EventHandler.OnDialogueStateChanged += OnDialogueStateChanged;
        EventHandler.OnAllDataReset += RefreshSceneState; // 监听数据重置事件

        // 监听场景解锁事件
        EventHandler.OnSceneUnlocked += OnSceneUnlocked;
        RefreshSceneState(); // 初始化状态

        //面板激活时再次确认隐藏
        if (mapPanel != null && mapPanel.activeSelf)
            mapPanel.SetActive(false);
    }

    private void OnDisable()
    {
        // 移除监听
        EventHandler.OnDialogueStateChanged -= OnDialogueStateChanged;
        EventHandler.OnAllDataReset -= RefreshSceneState;
        EventHandler.OnSceneUnlocked -= OnSceneUnlocked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !mapPanel.activeSelf && isEnabled)
        {
            // 切换面板显示状态
            ToggleMapPanel();
        }
        else if (mapPanel.activeSelf && Input.GetKeyDown(KeyCode.M))
        {
            ToggleMapPanel();
        }
    }

    /// <summary>切换地图面板状态（打开/关闭）</summary>
    public void ToggleMapPanel()
    {
        bool isActive = mapPanel.activeSelf;
        // 切换状态：显示→隐藏，隐藏→显示
        mapPanel.SetActive(!isActive);

        // 同步输入锁定状态：打开时锁定，关闭时解锁
        EventHandler.CallOnDialogueStateChanged(!isActive);
    }


    /// <summary>解锁指定场景</summary>
    private void OnSceneUnlocked(int sceneIndex)
    {
        if (sceneIndex == 2)
        {
            RefreshSceneState(); // 收到解锁事件后刷新状态
        }
    }

    /// <summary> 跳转场景1 </summary>
    private void JumpToScene1()
    {
        //判断当前场景是否为目标场景
        if (IsCurrentScene(scene1Name))
        {
            if (mapPanel.activeSelf)
                ToggleMapPanel(); // 是当前场景则只关闭面板
            return;
        }

        if (!string.IsNullOrEmpty(scene1Name))
        {
            JumpToScene(scene1Name);
        }
        else
        {
            Debug.LogError("场景一名称未配置！请在Inspector中设置scene1Name");
        }
    }

    /// <summary> 跳转至场景2 </summary>
    private void JumpToScene2()
    {
        // 判断当前场景是否为目标场景
        if (IsCurrentScene(scene2Name))
        {
            ToggleMapPanel(); // 是当前场景则只关闭面板
            return;
        }

        if (GameDataManager.Instance.IsSceneUnlocked(2))
        {
            //ToggleMapPanel();
            JumpToScene(scene2Name);
        }
        else
        {
            Debug.LogWarning("场景二尚未解锁！");
        }
    }

    /// <summary>通用场景跳转方法，包含场景验证</summary>
    private void JumpToScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("场景名称未配置！");
            return;
        }

        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"场景「{sceneName}」未添加到Build Settings！");
            return;
        }

        // 锁定输入防止重复操作
        if (mapPanel.activeSelf)
            ToggleMapPanel();

        Loader.SetTargetScene(sceneName);
    }



    /// <summary>验证场景是否在Build Settings中</summary>
    private bool IsSceneInBuildSettings(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string buildSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (buildSceneName.Equals(sceneName))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>判断当前激活场景是否为目标场景</summary>
    private bool IsCurrentScene(string targetSceneName)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return currentSceneName.Equals(targetSceneName);
    }

    /// <summary>刷新场景解锁状态（从数据管理器获取最新状态）</summary>
    private void RefreshSceneState()
    {

        if (scene2Button != null)
            scene2Button.interactable = GameDataManager.Instance.IsSceneUnlocked(2);

    }



    private void OnDialogueStateChanged(bool isDialogueActive)
    {
        isEnabled = !isDialogueActive;
    }

}
