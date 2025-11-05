using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NPCInteractionController : MonoBehaviour
{
    [Header("提示面板")]
    [SerializeField] private GameObject promptPanel;

    [Header("检测范围")]
    [SerializeField] private Collider2D interactionTrigger;

    [Header("对话关联（过渡剧情用）")]
    [SerializeField] private NPCDialogueSystem npcDialogueSystem; // 关联的对话组件
    [SerializeField] private ElvesUndeadDialogueEnd elvesUndeadDialogueEnd; // 关联的精灵亡者对话组件

    [Header("点击设置")]
    [SerializeField] private string targetTag = "NPC";//检测标签避免点击其他地方
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Camera mainCamera;

    private bool isPlayerInRange = false;  // 是否在检测范围内
    private bool isInteracting = false;    // 是否在对话
    private bool isEnabled = true;// 输入是否启用


    private void Awake()
    {
        // 提示UI默认是 false
        promptPanel.SetActive(false);


        //如果未赋值相机，自动获取主相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
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

    private void Update()
    {
        if (isEnabled && isPlayerInRange && promptPanel.activeSelf)
        {
            // 在检测范围内实时检测
            if (Input.GetKeyDown(KeyCode.E) && !isInteracting)
            {
                StartInteraction();
            }
            // 非探索场景不响应拾取
            if (!SceneManager.GetActiveScene().name.StartsWith(GameManager.Instance.exploreScenePrefix))
                return;

            // 检测鼠标左键点击
            if (Input.GetMouseButtonDown(0))
            {
                // 忽略UI点击（如果点击了按钮等UI元素，不执行销毁逻辑）
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
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            promptPanel.SetActive(true);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptPanel != null)
                promptPanel.SetActive(false);

            // 如果在对话就强制停止
            if (isInteracting)
            {
                EndInteraction();
            }
        }
    }

    /// <summary> 开始对话 </summary>
    public void StartInteraction()
    {
        //若未赋值对话组件，直接加载场景
        if (npcDialogueSystem == null)
        {
            return;
        }
        if (isInteracting) return;

        if (elvesUndeadDialogueEnd != null && !GameDataManager.Instance.IsElvenUndeadFirstMeeting() && elvesUndeadDialogueEnd.repeatDialogue != null)
        {
            npcDialogueSystem.repeatDialogue = elvesUndeadDialogueEnd.repeatDialogue;
        }
        else
        {
        }

        npcDialogueSystem.StartDialogueByEvent();//触发NPC对话

        isInteracting = true;//等待对话结束
        promptPanel.SetActive(false);

    }


    public void EndInteraction()
    {
        isInteracting = false;

        // 如果玩家还在范围内，重新显示交互提示（允许再次对话）
        if (isPlayerInRange)
        {
            promptPanel.SetActive(true);
        }
    }


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
            if (isInteracting)
            {
                EndInteraction();
            }
        }
    }
}
