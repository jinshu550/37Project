using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BodyType
{
    [Header("基础信息")]
    [Tooltip("卡牌唯一标识符")]
    public int cardId;

    [Tooltip("卡牌名称")]
    public string cardName;

    [Tooltip("卡牌身材")]
    public float cardBody;

    [Tooltip("卡牌描述")]
    [TextArea]
    public string description;

    [Tooltip("卡牌展示/战斗时的精灵")]
    public Sprite battleSprite;

}


