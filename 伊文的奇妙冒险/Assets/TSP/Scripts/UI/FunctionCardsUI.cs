using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FunctionCardsUI : Singleton<FunctionCardsUI>
{
    [SerializeField] List<GameObject> imageSlots = new();

    // 每种功能牌类型对应的Sprite（在Inspector中赋值）
    [SerializeField] private Sprite BreathofGaleSprite;
    [SerializeField] private Sprite ElfArrowSprite;
    [SerializeField] private Sprite GoldenFruitSprite;
    [SerializeField] private Sprite GoldenSpecialBlendSprite;
    [SerializeField] private Sprite MoonlightGrassSprite;
    [SerializeField] private Sprite NightDewWaterSprite;
    [SerializeField] private Sprite TreantShieldSprite;
    [SerializeField] private Sprite BanedSlot;

    // 存储已生成的UI，避免重复（key：功能牌类型）
    private Dictionary<FunctionCardType, Sprite> spawnedUIs = new();
    //记录每一个slot是否被占用
    private List<bool> slotOccupied = new();

    // 初始化：生成所有功能牌UI
    public void InitFunctionCardsUI()
    {
        //初始化slot
        slotOccupied.Clear();
        for (int i = 0; i < imageSlots.Count; i++)
        {
            slotOccupied.Add(false);
        }
        //清空
        ClearAllUI();
        // 从CardSystem获取功能牌列表
        foreach (var card in CardSystem.Instance.functionCards)
        {
            SpawnCardUI(card);
        }
    }

    // 生成单张功能牌的UI
    private void SpawnCardUI(Card card)
    {
        FunctionCardType cardType = card.FunctionCardType;

        // 避免重复生成同一类型的UI（如果需要显示数量，可在此处修改逻辑）
        if (spawnedUIs.ContainsKey(cardType))
        {
            // 若允许同类型多张，可移除此判断，或更新数量显示
            return;
        }
        //找到第一个未被占用的slot
        int emptySlotIndex = -1;
        for (int i = 0; i < slotOccupied.Count; i++)
        {
            if (!slotOccupied[i])
            {
                emptySlotIndex = i;
                break;
            }
        }
        if (emptySlotIndex != -1)
        {
            GameObject slot = imageSlots[emptySlotIndex];
            Image image = slot.GetComponent<Image>();
            image.sprite = GetSpriteByType(cardType);
            slotOccupied[emptySlotIndex] = true;
            spawnedUIs.Add(cardType, image.sprite);
        }
    }

    // 根据类型获取Sprite（复用你的方法）
    private Sprite GetSpriteByType(FunctionCardType functionCardType)
    {
        return functionCardType switch
        {
            FunctionCardType.BreathofGale => BreathofGaleSprite,
            FunctionCardType.ElfArrow => ElfArrowSprite,
            FunctionCardType.GoldenFruit => GoldenFruitSprite,
            FunctionCardType.GoldenSpecialBlend => GoldenSpecialBlendSprite,
            FunctionCardType.MoonlightGrass => MoonlightGrassSprite,
            FunctionCardType.NightDewWater => NightDewWaterSprite,
            FunctionCardType.TreantShield => TreantShieldSprite,
            _ => null
        };
    }

    // 清空所有UI
    private void ClearAllUI()
    {
        for (int i = 0; i < imageSlots.Count; i++)
        {
            Image image = imageSlots[i].GetComponent<Image>();
            image.sprite = BanedSlot;
            slotOccupied[i] = false;
        }
        spawnedUIs.Clear();
    }

    // 动态添加功能牌UI（如果后期新增功能牌）
    public void AddFunctionCardUI(Card newCard)
    {
        SpawnCardUI(newCard);
    }

    // 动态移除功能牌UI（如果后期移除功能牌）
    public void RemoveFunctionCardUI(FunctionCardType type)
    {
        if (spawnedUIs.TryGetValue(type, out var ui))
        {
            // 找到该UI对应的slot索引
            int slotIndex = -1;
            for (int i = 0; i < imageSlots.Count; i++)
            {
                if (imageSlots[i].GetComponent<Image>() == ui)
                {
                    slotIndex = i;
                    break;
                }
            }
            if (slotIndex != -1)
            {
                // 还原为BanedSlotSprite
                ui = BanedSlot;
                // 标记该slot为未占用
                slotOccupied[slotIndex] = false;
                spawnedUIs.Remove(type);
            }
        }
    }
}
