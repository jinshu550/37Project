using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFormationManager : MonoBehaviour
{
    [Header("出战配置")]
    [SerializeField] private int maxBattleCards = 3; // 最大出战数量
    private List<BackCardUI> battleCards = new List<BackCardUI>(); // 出战列表

    private List<BackCardUI> currentSelectedCards; //当前多选的

    private void OnEnable()
    {
        EventHandler.OnSelectionChanged += OnSelectionChanged;
        //场景加载/面板激活时，从GameDataManager恢复出战状态
        RestoreBattleStateFromDataManager();
    }

    private void OnDisable()
    {
        EventHandler.OnSelectionChanged -= OnSelectionChanged;
        currentSelectedCards?.Clear();
    }

    private void OnSelectionChanged(bool isSingleMode, BackCardUI singleCard, List<BackCardUI> multiCards)
    {
        if (!isSingleMode)
        { // 仅关注多选（出战需多选卡牌）
            currentSelectedCards = multiCards;
        }
    }

    /// <summary> 处理出战按钮点击（由BackpackUIManager调用） </summary>
    public void OnBattleButtonClick()
    {
        if (currentSelectedCards == null || currentSelectedCards.Count == 0) return;

        // 检查多选列表中是否所有卡牌都已出战
        bool allInBattle = true;
        foreach (var card in currentSelectedCards)
        {
            BackCardUI result = battleCards.Find((BackCardUI c) => c.GetCardName() == card.GetCardName());
            if (result == null)
            {
                allInBattle = false;
                break;
            }
        }
        if (!allInBattle)
        {
            // 存在未出战的卡牌，将所有选中卡牌设为出战
            foreach (var card in currentSelectedCards)
            {
                // 只添加不在出战列表中的卡牌
                if (!battleCards.Contains(card))
                {
                    AddToBattleList(card);
                }
            }
        }
        else
        {
            // 批量从出战列表移除
            foreach (var card in currentSelectedCards)
            {
                RemoveFromBattleList(card);
            }
        }
        // 保存当前出战状态到数据层
        SaveBattleStateToDataManager();

    }

    /// <summary> 添加卡牌到出战列表
    /// </summary>
    /// <param name="card">需要添加的卡牌</param>
    private void AddToBattleList(BackCardUI card)
    {
        // 如果已满3个，移除第一个
        if (battleCards.Count >= maxBattleCards)
        {
            BackCardUI firstCard = battleCards[0];
            RemoveFromBattleList(firstCard);
        }

        // 添加新卡牌
        battleCards.Add(card);
        card.SetBattleState(true); // 通知卡牌显示出战状态
    }

    /// <summary> 从出战列表移除卡牌
    /// </summary>
    /// <param name="card">需要取消的卡牌</param>
    private void RemoveFromBattleList(BackCardUI card)
    {
        if (battleCards.Remove(card))
        {
            card.SetBattleState(false); // 通知卡牌隐藏出战状态
        }
    }


    /// <summary> 从GameDataManager读取数据，恢复UI层的出战状态 </summary>
    public void RestoreBattleStateFromDataManager()
    {
        if (GameDataManager.Instance == null) return;
        // 清空当前UI层列表
        battleCards.Clear();
        // 获取 GameDataManager 中的出战卡牌 名字
        List<string> savedBattleIds = GameDataManager.Instance.GetBattleCardINames();
        if (savedBattleIds.Count == 0) return;

        // 找到场景中所有BackCardUI，匹配ID后恢复出战状态
        BackCardUI[] allCardsInScene = FindObjectsOfType<BackCardUI>();
        foreach (var card in allCardsInScene)
        {
            if (savedBattleIds.Contains(card.GetCardName()))
            {
                battleCards.Add(card);
                card.SetBattleState(true); // 恢复出战视觉状态
            }
        }
    }

    /// <summary> 将出战列表 添加到 RestoreBattleStateFromDataManager 中 </summary>
    private void SaveBattleStateToDataManager()
    {
        //打开界面
        //关闭界面
        if (GameDataManager.Instance == null) return;

        //先清空数据层列表
        GameDataManager.Instance.ClearBattleCards();
        // if (StaticUtility.BattleCards != null)
        // {
        //     StaticUtility.BattleCards.Clear();
        // }
        //遍历出战列表将数据保存
        foreach (var card in battleCards)
        {
            GameDataManager.Instance.AddBattleCard(card.GetCardName());
        }
        // for (int i = 0; i < battleCards.Count; i++)
        // {
        //     //需要少平改代码
        //     StaticUtility.BattleCards.Add(battleCards[i].GetCardName());
        // }
    }
}
