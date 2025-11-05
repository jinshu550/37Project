using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonFollowObject : MonoBehaviour
{
    [Header("物体设置")]
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Transform originalParent;  //原来的父物体
    [SerializeField] private Vector3 offsetPos = Vector3.zero;  //原来的父物体

    [Header("按钮设置")]
    [SerializeField] private Transform[] targetButtons;
    private Transform currentHoverButton = null;  // 当前悬停的按钮

    private EventSystem eventSystem;

    private PointerEventData pointerData;   //指针数据

    void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            Debug.LogWarning("场景中未找到 EventSystem，已自动创建");
        }
        // 初始化指针数据
        pointerData = new PointerEventData(eventSystem);

        // 确保核心参数已赋值
        if (targetObject == null)
        {
            Debug.LogError("请在 Inspector 中赋值「targetObject」（需要移动的物体）！");
            enabled = false; // 禁用脚本，避免报错
            return;
        }

        if (originalParent == null)
        {
            // 若未赋值初始父物体，默认使用目标物体当前的父物体
            originalParent = targetObject.transform.parent;
            Debug.LogWarning("未赋值「originalParent」，已自动使用目标物体当前父物体：" + originalParent.name);
        }
        if (targetButtons == null || targetButtons.Length != 4)
        {
            Debug.LogError("请在 Inspector 中赋值「targetButtons」，且必须包含 4 个按钮数据！");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        // 每帧检测鼠标悬停的按钮
        CheckMouseHoverButton();
        // 根据悬停状态，切换目标物体的父物体
        UpdateTargetObjectParent();
    }

    /// <summary>  检测鼠标当前悬停的按钮 </summary>
    private void CheckMouseHoverButton()
    {
        // 更新指针位置
        pointerData.position = Input.mousePosition;
        // 重置当前悬停按钮
        currentHoverButton = null;

        //检测按钮
        if (eventSystem.IsPointerOverGameObject()) //判断是否在UI对象上
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // 遍历UI检测结果，判断是否命中目标按钮
            foreach (var result in results)
            {
                foreach (var button in targetButtons)
                {
                    if (result.gameObject == button?.gameObject)
                    {
                        currentHoverButton = button;
                        return; // 找到匹配的按钮，直接返回
                    }
                }
            }
        }
    }

    /// <summary>  根据鼠标悬停状态，更新目标物体的父物体 </summary>
    private void UpdateTargetObjectParent()
    {
        // 鼠标悬停在某个按钮上
        if (currentHoverButton != null)
        {
            if (targetObject.transform.parent != currentHoverButton)
            {
                targetObject.transform.SetParent(currentHoverButton);// 让物体在目标父物体下
                targetObject.transform.localPosition = offsetPos;
                targetObject.SetActive(true);

            }
        }
        // 鼠标移开，回归原父物体
        else
        {
            if (originalParent != null && targetObject.transform.parent != originalParent)
            {
                targetObject.transform.SetParent(originalParent);// 回归原父物体下的初始位置
                targetObject.transform.localPosition = offsetPos + new Vector3(-1000, 0, 0);
                targetObject.SetActive(false);

            }
        }
    }


}



