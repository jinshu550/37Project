using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardBasicDatabase", menuName = "Inventory/Card Basic Database", order = 1)]
public class CardBasicDatabase : ScriptableObject
{
    [Header("扩展属性卡牌列表")]
    [Tooltip("在此添加带扩展属性的卡牌（如功能牌、可合成牌，含地图精灵、合成配方）")]
    public List<CardBasicInformation> allExtendedCards = new List<CardBasicInformation>();

    // 缓存：CardId -> CardBasicInformation（快速查询）
    private Dictionary<int, CardBasicInformation> _extendedCardDict = new Dictionary<int, CardBasicInformation>();

    /// <summary>
    /// 数据库激活时初始化字典
    /// </summary>
    private void OnEnable()
    {
        InitializeExtendedCardDictionary();
    }

    /// <summary>
    /// 初始化扩展卡牌字典，处理空数据与ID重复
    /// </summary>
    public void InitializeExtendedCardDictionary()
    {
        _extendedCardDict.Clear();

        foreach (var extendedCard in allExtendedCards)
        {
            if (extendedCard == null)
            {
                Debug.LogWarning("扩展属性卡牌列表中存在空数据，请检查！");
                continue;
            }

            if (!_extendedCardDict.ContainsKey(extendedCard.cardId))
            {
                _extendedCardDict.Add(extendedCard.cardId, extendedCard);
            }
            else
            {
                Debug.LogError($"扩展属性卡牌ID重复！ID: {extendedCard.cardId}，重复卡牌名称: {extendedCard.cardName}");
            }
        }

    }

    /// <summary>
    /// 根据 CardId 查询扩展属性卡牌（对外公开接口）
    /// </summary>
    /// <param name="cardId">卡牌唯一ID</param>
    /// <returns>对应的CardBasicInformation卡牌（无则返回null）</returns>
    public CardBasicInformation GetExtendedCardById(int cardId)
    {
        if (_extendedCardDict.TryGetValue(cardId, out var targetCard))
        {
            return targetCard;
        }
        return null;
    }

    /// <summary>
    /// 批量查询：根据多个CardId获取扩展卡牌列表（如背包批量加载,或者还有合成配方的查找）
    /// </summary>
    public List<CardBasicInformation> GetExtendedCardsByIds(List<int> cardIds)
    {
        List<CardBasicInformation> result = new List<CardBasicInformation>();

        if (cardIds == null || cardIds.Count == 0)
        {
            Debug.LogWarning("传入的CardId列表为空，无法查询扩展卡牌");
            return result;
        }

        foreach (var id in cardIds)
        {
            var card = GetExtendedCardById(id);
            if (card != null)
            {
                result.Add(card);
            }
        }

        return result;
    }

}
