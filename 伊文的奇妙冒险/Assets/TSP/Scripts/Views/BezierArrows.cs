using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基于贝塞尔曲线的箭头控制器，实现箭头从起点平滑指向目标位置
/// </summary>
public class BezierArrows : MonoBehaviour
{
    #region 公共字段
    [Tooltip("箭头头部预制体")]
    public GameObject ArrowHeadPrefab;

    [Tooltip("箭头节点预制体")]
    public GameObject ArrowNodePrefab;

    [Tooltip("箭头节点数量")]
    public int arrowNodeNum;

    [Tooltip("箭头节点缩放系数")]
    public float scaleFactor = 1f;

    [Tooltip("显示对象的父节点（请勿手动赋值）")]
    public Transform ShowObj;

    [Tooltip("是否激活显示箭头（请勿手动赋值）由ManualTargetingSystem控制")]
    public bool IsActive = false;

    [Tooltip("箭头起点位置（请勿手动赋值）由ManualTargetingSystem控制")]
    public Vector2 Position = Vector2.zero;
    #endregion

    #region 私有字段
    /// <summary>
    /// 箭头发射点的RectTransform（P0位置）
    /// </summary>
    private RectTransform origin;

    /// <summary>
    /// 存储所有箭头节点的RectTransform列表（包含箭头头部）
    /// </summary>
    private List<RectTransform> arrowNodes = new List<RectTransform>();

    /// <summary>
    /// 贝塞尔曲线的控制点列表（P0-P3）
    /// </summary>
    private List<Vector2> controlPoints = new List<Vector2>();

    /// <summary>
    /// 控制点P1、P2的计算因子，用于控制曲线形态
    /// </summary>
    private readonly List<Vector2> controlPointFactors = new List<Vector2>()
    {
        new Vector2(-0.3f, 0.8f),
        new Vector2(0.1f, 1.4f)
    };
    #endregion

    #region 生命周期方法
    /// <summary>
    /// 初始化时执行，创建箭头节点和控制点
    /// </summary>
    private void Start()
    {
        // 获取箭头发射点的RectTransform
        origin = GetComponent<RectTransform>();

        // 创建箭头节点
        for (int i = 0; i < arrowNodeNum; i++)
        {
            GameObject arrowNode = Instantiate(ArrowNodePrefab, ShowObj);
            arrowNode.SetActive(false);
            arrowNode.transform.SetAsFirstSibling();
            arrowNodes.Add(arrowNode.GetComponent<RectTransform>());
        }

        // 创建箭头头部并添加到节点列表
        GameObject arrowHead = Instantiate(ArrowHeadPrefab, ShowObj);
        arrowHead.SetActive(false);
        arrowHead.transform.SetAsFirstSibling();
        arrowNodes.Add(arrowHead.GetComponent<RectTransform>());

        // 初始化时将所有节点隐藏到屏幕外
        foreach (var node in arrowNodes)
        {
            node.position = new Vector2(-1000, -1000);
        }

        // 初始化4个贝塞尔曲线控制点
        for (int i = 0; i < 4; i++)
        {
            controlPoints.Add(Vector2.zero);
        }

    }
    public void SetupArrow(Vector3 startPosition)
    {
        Position = startPosition;
        IsActive = true;
    }

    /// <summary>
    /// 每帧更新箭头位置和形态
    /// </summary>
    private void Update()
    {
        UpdateControlPoints();
        UpdateArrowNodes();
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 更新贝塞尔曲线的控制点
    /// </summary>
    private void UpdateControlPoints()
    {
        // 设置起点P0
        controlPoints[0] = Position;

        // 获取鼠标位置作为终点P3
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        controlPoints[3] = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        // 计算中间控制点P1和P2
        Vector2 delta = controlPoints[3] - controlPoints[0];
        controlPoints[1] = controlPoints[0] + delta * controlPointFactors[0];
        controlPoints[2] = controlPoints[0] + delta * controlPointFactors[1];
    }

    /// <summary>
    /// 更新所有箭头节点的位置、旋转和显示状态
    /// </summary>
    private void UpdateArrowNodes()
    {
        int nodeCount = arrowNodes.Count;

        for (int i = 0; i < nodeCount; i++)
        {
            RectTransform node = arrowNodes[i];

            // 计算当前节点在贝塞尔曲线上的位置
            float t = CalculateTValue(i, nodeCount);
            node.position = CalculateBezierPoint(t);

            // 设置节点旋转（除第一个节点外）
            if (i > 0)
            {
                SetNodeRotation(node, arrowNodes[i - 1]);
            }

            // 设置节点显示状态
            node.gameObject.SetActive(IsActive);

            // 计算节点缩放（当前注释掉，如需使用可取消注释）
            // SetNodeScale(node, i, nodeCount);
        }

        // 同步第一个节点与第二个节点的旋转
        if (nodeCount > 1)
        {
            arrowNodes[0].rotation = arrowNodes[1].rotation;
        }
    }

    /// <summary>
    /// 计算贝塞尔曲线的参数t值
    /// </summary>
    private float CalculateTValue(int index, int totalCount)
    {
        return Mathf.Log(1f * index / (totalCount - 1) + 1f, 2f);

    }
    /// <summary>
    /// 根据三次贝塞尔曲线公式计算点位置
    /// </summary>
    private Vector2 CalculateBezierPoint(float t)
    {
        float mt = 1 - t;
        float mt2 = mt * mt;
        float mt3 = mt2 * mt;
        float t2 = t * t;
        float t3 = t2 * t;

        return mt3 * controlPoints[0]
                + 3 * mt2 * t * controlPoints[1]
                + 3 * mt * t2 * controlPoints[2]
                + t3 * controlPoints[3];

    }

    /// <summary>
    /// 设置节点旋转角度，使其沿曲线方向
    /// </summary>
    private void SetNodeRotation(RectTransform currentNode, RectTransform previousNode)
    {
        Vector2 direction = currentNode.position - previousNode.position;
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        currentNode.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// 设置节点缩放（当前注释掉，如需使用可取消注释）
    /// </summary>
    private void SetNodeScale(RectTransform node, int index, int totalCount)
    {
        float scale = scaleFactor * (1f - 0.03f * (totalCount - 1 - index));
        node.localScale = new Vector3(scale, scale, 1f);
    }
    #endregion
}