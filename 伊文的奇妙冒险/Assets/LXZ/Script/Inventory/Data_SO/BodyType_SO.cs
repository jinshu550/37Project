using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BodyType_SO", menuName = "Inventory/BodyType_SO", order = 1)]

public class BodyType_SO : ScriptableObject
{
    [Tooltip("基础身材卡牌列表")]
    public List<BodyType> allBodyCards = new List<BodyType>();

    private Dictionary<int, BodyType> _bodyCardDict = new Dictionary<int, BodyType>();

    /// <summary>
    /// 数据库激活时初始化字典（如进入游戏、Inspector切换时）
    /// </summary>
    private void OnEnable()
    {
        InitializeCardDictionary();
    }

    /// <summary>
    /// 初始化字典：将列表转为键值对，处理ID重复问题
    /// </summary>
    private void InitializeCardDictionary()
    {
        _bodyCardDict.Clear(); // 先清空，避免重复添加

        foreach (var bodyCard in allBodyCards)
        {
            // 跳过空数据，避免空引用
            if (bodyCard == null)
            {
                Debug.LogWarning("基础身材卡牌列表中存在空数据，请检查！");
                continue;
            }

            // 检查ID是否重复（重复ID会导致查询异常）
            if (!_bodyCardDict.ContainsKey(bodyCard.cardId))
            {
                _bodyCardDict.Add(bodyCard.cardId, bodyCard);
            }
            else
            {
                Debug.LogError($"基础身材卡牌ID重复！ID: {bodyCard.cardId}，重复卡牌名称: {bodyCard.cardName}");
            }
        }

    }


    /// <summary>
    /// 获取所有基础身材卡牌（用于全量展示，如图鉴）
    /// </summary>
    public List<BodyType> GetAllBodyCards()
    {
        return new List<BodyType>(allBodyCards); // 返回副本，避免外部修改原列表
    }

}
