using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // 需要引用场景中的Canvas（确保Canvas上有GraphicRaycaster组件）
    public Canvas canvas;

    // 用于检测世界物体的相机（通常是主相机）
    public Camera worldCamera;
    public LayerMask targetLayer;

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;

    void Start()
    {
        // 获取Canvas上的GraphicRaycaster组件（UI射线检测核心）
        if (canvas != null)
        {
            raycaster = canvas.GetComponent<GraphicRaycaster>();
        }
        else
        {
            // 如果没指定Canvas，尝试查找场景中第一个Canvas
            canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
                raycaster = canvas.GetComponent<GraphicRaycaster>();
            else
                Debug.LogError("场景中没有找到Canvas！");
        }

        // 如果没有指定世界相机，尝试获取主相机
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
            if (worldCamera == null)
                Debug.LogError("场景中没有找到主相机！");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && raycaster != null)
        {

            // 先检测UI元素
            bool isUIClicked = CheckUIClick();

            // 如果没有点击到UI，再检测世界物体
            if (!isUIClicked)
            {
                CheckWorldObjectClick();
            }
        }
    }

    /// <summary>
    /// 检测UI点击
    /// </summary>
    /// <returns>是否点击到UI</returns>
    private bool CheckUIClick()
    {
        // 创建射线检测数据
        pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        // 存储射线命中的UI元素
        List<RaycastResult> results = new List<RaycastResult>();
        // 执行射线检测
        raycaster.Raycast(pointerEventData, results);

        // 处理检测结果
        if (results.Count > 0)
        {
            // 取第一个命中的UI元素（最上层的）
            GameObject clickedUI = results[0].gameObject;
            Debug.Log("点击到的UI层级: " + clickedUI.transform.GetPathName(""));
            return true;
        }
        else
        {

        }

        return false;
    }

    /// <summary>
    /// 检测世界物体点击
    /// </summary>
    private void CheckWorldObjectClick()
    {
        // 从相机发射射线到鼠标位置
            Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);
            // 转换为2D射线检测（检测2D碰撞体）
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, targetLayer);

        // 检测2D碰撞体
        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;
            Debug.Log("点击到的2D物体: " + clickedObject.name + "，层级路径: " + clickedObject.transform.GetPathName(""));
        }
        else
        {
            Debug.Log("没有点击到任何UI元素或2D物体");
        }
    }
}
