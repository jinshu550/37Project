using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapInteraction : MonoBehaviour
{
    [Header("提示面板")]
    [SerializeField] private GameObject promptPanel;

    [Header("点击设置")]
    [SerializeField] private string targetTag = "Collect";//检测标签避免点击其他地方
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Camera mainCamera;

    private bool isPlayerInRange = false;  // 是否在检测范围内
    private bool isUnlockCompleted = false; // 场景二解锁流程是否完成
    private bool isPopupShowing = false;  // 弹窗是否正在显示


    private void Awake()
    {
        //如果未赋值相机，自动获取主相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

    }

    void Update()
    {
        if (isUnlockCompleted || isPopupShowing)
            return;

        // 非探索场景不响应拾取
        if (!SceneManager.GetActiveScene().name.StartsWith(GameManager.Instance.exploreScenePrefix))
            return;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {

            CompleteScene2Unlock();
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
                    CompleteScene2Unlock();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 已解锁/弹窗显示时，不激活提示面板
        if (isUnlockCompleted || isPopupShowing||GameDataManager.Instance.IsSceneUnlocked(2))
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
        // 已解锁/弹窗显示时，不激活提示面板
        if (isUnlockCompleted || isPopupShowing)
            return;

        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptPanel != null)
                promptPanel.SetActive(false);
        }
    }

    /// <summary>完成交互并解锁场景二</summary>
    private void CompleteScene2Unlock()
    {
        if (isUnlockCompleted)
            return;

        // 标记解锁流程完成（避免重复执行）
        isUnlockCompleted = true;

        // 通知数据管理器解锁场景二
        GameDataManager.Instance.UnlockScene(2);

        // 销毁提示面板
        if (promptPanel != null)
            promptPanel.SetActive(false);

        // 禁用自身碰撞体（防止玩家再次触发）
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        //触发地图显示按钮
        GameManager.Instance.OpenMap();
    }

}
