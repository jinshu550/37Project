using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardBasicInformation : BodyType
{
    [Header("资源引用")]
    [Tooltip("地图上显示的精灵")]
    public Sprite mapSprite;

    [Header("合成信息")]
    [Tooltip("合成此卡牌所需的其他卡牌ID")]
    public List<int> requiredCardIds;
    [field: SerializeField] public CardData card;
    // 检查是否可以合成此卡牌
    public bool CanSynthesize(List<int> ownedCardIds)
    {
        if (requiredCardIds == null || requiredCardIds.Count == 0)
            return false;

        foreach (int requiredId in requiredCardIds)
        {
            if (!ownedCardIds.Contains(requiredId))
                return false;
        }
        return true;
    }
}


