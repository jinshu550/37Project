using System.Collections.Generic;
using UnityEngine;
public class Card
{
    public readonly CardData data;
    // 新增：静态属性记录当前正在打出的卡牌
    public static Card CurrentPlayingCard { get; set; }
    public string Title => data.Name;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public Effect ManualTargetEffect => data.ManualTargetEffect;
    public List<AutoTargetEffect> OtherEffects => data.OtherEffects;
    public FunctionCardType FunctionCardType { get; private set; }
    public int Mana { get; private set; }
    public Card(CardData cardData)
    {
        data = cardData;
        if (cardData.Mana != 0)
            Mana = cardData.Mana;
        FunctionCardType = data.FunctionCardType;
    }

}