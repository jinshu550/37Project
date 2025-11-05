using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BattleSceneSwitcher : MonoBehaviour
{
    [Header("切换的场景设置")]
    [SerializeField] private string enemyName;
    [SerializeField] private string battleSceneName = "Fight_Scene";
    [SerializeField] private bool hasTransitionStory = false; //是否需要过渡剧情

    [Header("需要生成的怪物数据")]
    [SerializeField] private List<EnemyData> enemiesToSpawn;


    [Header("提示面板")]
    [SerializeField] private GameObject promptPanel;

    [Header("点击设置")]
    [SerializeField] private string targetTag = "Collect";//检测标签避免点击其他地方
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Camera mainCamera;

    [Header("对话关联（过渡剧情用）")]
    [SerializeField] private NPCDialogueSystem npcDialogueSystem; // 关联的对话组件
    [SerializeField] private DialogueLine[] beforeUnlockedDialogue; // 背包解锁背包前的对话
    [SerializeField] private DialogueLine[] afterUnlockedDialogue;  // 解锁背包后的对话
    [SerializeField] private DialogueLine[] originalDialogue;  // 解锁背包后的对话
    private bool isWaitingForDialogueEnd = false; // 是否处于 等待对话结束状态

    private bool isPlayerInRange = false;  // 是否在检测范围内
    private static string previousSceneName;//记录切换前的探索场景名（用于返回）
    private bool isEnabled = true;// 输入是否启用
    private bool isFirstDialogue = true; //是否为第一次对话

    private void Awake()
    {
        //如果未赋值相机，自动获取主相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        // 初始化对话内容
        InitializeDialogueContent();
    }

    private void OnEnable()
    {
        EventHandler.OnDialogueStateChanged += OnDialogueStateChanged;// 监听对话状态变化事件

        EventHandler.OnBackpackUnlocked += OnBackpackUnlocked; // 监听背包解锁事件
    }

    private void OnDisable()
    {
        EventHandler.OnDialogueStateChanged -= OnDialogueStateChanged;

        EventHandler.OnBackpackUnlocked -= OnBackpackUnlocked;
    }

    void Update()
    {
        if (!isEnabled)
            return;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SwitchBattleScene();
        }

        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0) && isPlayerInRange)
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
                    SwitchBattleScene();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检测是否是玩家
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (promptPanel != null)
                promptPanel.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 检测是否是玩家
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptPanel != null)
                promptPanel.SetActive(false);
        }
    }

    /// <summary> 切换到战斗场景 </summary>
    private void SwitchBattleScene()
    {

        //检查背包是否解锁
        if (!GameDataManager.Instance.HasBackpackUnlocked())
        {
            isWaitingForDialogueEnd = true;//等待对话结束
            npcDialogueSystem.StartDialogueByEvent();//触发NPC对话
            return; // 终止后续切换逻辑
        }

        //检查敌人数据
        if (npcDialogueSystem.npcName == "精灵亡者")
        {
            //检查是否为首次触发
            if (npcDialogueSystem.GetIsFirstMeeting())
            {
                isWaitingForDialogueEnd = true;//等待对话结束
                npcDialogueSystem.StartDialogueByEvent();//触发NPC对话
                isFirstDialogue = false;
                return; // 终止后续切换逻辑  前面return就不会进行战斗，这个return就会进入 为什么
            }
        }

        if (hasTransitionStory)
        {
            //若未赋值对话组件，直接加载场景
            if (npcDialogueSystem == null)
            {
                SavePos();//保存位置
                //传递怪物数据
                StaticUtility.CurrentEnemyData = enemiesToSpawn;
                Loader.SetTargetScene(battleSceneName);
                return;
            }
            isWaitingForDialogueEnd = true;//等待对话结束
            npcDialogueSystem.StartDialogueByEvent();//触发NPC对话
        }
        else
        {
            SavePos();//保存位置
            StaticUtility.CurrentEnemyData = enemiesToSpawn; //传递怪物数据
            Loader.SetTargetScene(battleSceneName);
        }
    }


    #region 对话相关
    /// <summary>  背包解锁时更新对话内容  </summary>
    private void OnBackpackUnlocked()
    {
        InitializeDialogueContent();
    }

    /// <summary> 根据背包状态初始化对话内容 </summary>
    private void InitializeDialogueContent()
    {
        if (npcDialogueSystem == null) return;

        bool isUnlocked = GameDataManager.Instance.HasBackpackUnlocked();

        if (npcDialogueSystem.repeatDialogue != beforeUnlockedDialogue)
            originalDialogue = npcDialogueSystem.repeatDialogue;

        // 根据背包状态设置对话内容
        if (isUnlocked)
        {
            // 解锁后使用指定的对话
            npcDialogueSystem.firstMeetingDialogue = afterUnlockedDialogue;
            npcDialogueSystem.repeatDialogue = originalDialogue;
        }
        else
        {
            // 未解锁时使用提示解锁的对话
            npcDialogueSystem.firstMeetingDialogue = beforeUnlockedDialogue;
            npcDialogueSystem.repeatDialogue = beforeUnlockedDialogue;
        }
    }

    /// <summary> 对话状态变化回调
    /// </summary>
    /// <param name="isDialogueActive"></param>
    private void OnDialogueStateChanged(bool isDialogueActive)
    {
        // 仅当“等待对话结束”且“对话已结束”时，才加载战斗场景
        if (isWaitingForDialogueEnd && !isDialogueActive)
        {
            isWaitingForDialogueEnd = false; // 重置等待标记
            switch (enemyName)
            {
                case "稻草人":
                    if (GameDataManager.Instance.HasBackpackUnlocked())
                    {
                        SavePos();//保存位置
                        StaticUtility.CurrentEnemyData = enemiesToSpawn; //传递怪物数据
                        Loader.SetTargetScene(battleSceneName); // 对话结束，并解锁了背包 才切换到加载场景后切战斗场景
                    }
                    break;
                case "精灵亡者":
                    if (!GameDataManager.Instance.IsElvenUndeadFirstMeeting())
                    {
                        if (GameDataManager.Instance.HasBackpackUnlocked())
                        {
                            if (StaticUtility.CurrentEnemyBuff != null)
                            {
                                SavePos();//保存位置
                                StaticUtility.CurrentEnemyData = enemiesToSpawn; //传递怪物数据
                                Loader.SetTargetScene(battleSceneName); // 对话结束，并解锁了背包 才切换到加载场景后切战斗场景
                            }

                        }
                    }

                    break;
                default:
                    StaticUtility.CurrentEnemyBuff = null;
                    break;
            }

        }

        if (GameDataManager.Instance.IsElvenUndeadFirstMeeting() && !isFirstDialogue)
        {
            GameDataManager.Instance.SaveElvenUndeadFirstMeeting(false);
        }
    }
    #endregion

    /// <summary> 战斗结束返回原探索场景 </summary>
    public static void ReturnToPreviousScene()
    {
        StaticUtility.ClearPartData();
        ActionSystem.ClearAllSubscriptions();
        // 若未记录原场景，默认加载最后保存的探索场景
        if (string.IsNullOrEmpty(previousSceneName))
        {
            previousSceneName = GameDataManager.Instance.GetSavedSceneName();
        }

        // 验证场景有效性并加载
        bool isSceneValid = !string.IsNullOrEmpty(previousSceneName) && previousSceneName.StartsWith(GameManager.Instance.exploreScenePrefix);

        if (isSceneValid)
        {
            Loader.SetTargetScene(previousSceneName);
        }
        else
        {
            Loader.SetTargetScene($"{GameManager.Instance.exploreScenePrefix}001Scene");
        }
    }

    //保存角色位置
    private void SavePos()
    {
        previousSceneName = SceneManager.GetActiveScene().name;//记录当前探索场景名（切换前）
        //优先保存当前场景的角色位置
        string currentSceneName = SceneManager.GetActiveScene().name;
        GameObject player = GameManager.Instance?.GetCurrentPlayerTransform()?.gameObject;
        if (player != null && currentSceneName.StartsWith(GameManager.Instance.exploreScenePrefix))
        {
            GameDataManager.Instance.SavePlayerPosition(currentSceneName, player.transform.position);
            if (currentSceneName == "Explore_002Scene" && CameraFollow.instance.boundary != null)
            {
                // 存入需要加载的bound名称
                GameDataManager.Instance.SaveCurrentBound(CameraFollow.instance.boundary.name);
            }
        }
        else
        {
        }
    }

}