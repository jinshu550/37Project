using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;
    [SerializeField] private int maxMultiSelections = 3;  // 多选最大数量

    private bool isSingleSelectionMode = true; // 默认单选
    private BackCardUI singleSelectedCard;   //单选模式的选中项
    private List<BackCardUI> multiSelectedCards = new List<BackCardUI>();//多选模式的选中项

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>设置选择模式（单选/多选）
    /// </summary>
    /// <param name="isSingleMode"></param>
    public void SetSelectionMode(bool isSingleMode)
    {
        // 如果模式切换了就清楚选中状态
        if (isSingleSelectionMode != isSingleMode)
        {
            ClearAllSelections();
        }
        isSingleSelectionMode = isSingleMode;
        // 触发全局事件
        EventHandler.CallOnSelectionChanged(isSingleSelectionMode, singleSelectedCard, multiSelectedCards);
    }

    /// <summary>处理卡牌选中事件（由BackCardUI调用）
    /// </summary>
    /// <param name="selectedCard"></param>
    public void OnCardSelected(BackCardUI selectedCard)
    {
        if (isSingleSelectionMode)
        {
            HandleSingleSelection(selectedCard);
        }
        else
        {
            HandleMultiSelection(selectedCard);
        }
        // 触发全局事件
        EventHandler.CallOnSelectionChanged(isSingleSelectionMode, singleSelectedCard, multiSelectedCards);
    }

    /// <summary>单选逻辑
    /// </summary>
    /// <param name="selectedCard"></param>
    private void HandleSingleSelection(BackCardUI selectedCard)
    {
        //点击的是已选中的卡牌 就会取消选中
        if (singleSelectedCard == selectedCard)
        {
            singleSelectedCard.SetSelectedState(false);
            singleSelectedCard.cardButton.interactable = true;
            singleSelectedCard = null;
            return;
        }

        //如果没有选中就会选中  并会取消之前的选中
        if (singleSelectedCard != null)
        {
            singleSelectedCard.SetSelectedState(false);
            singleSelectedCard.cardButton.interactable = true;
        }

        // 更新选中当前项
        singleSelectedCard = selectedCard;
        singleSelectedCard.SetSelectedState(true);
        singleSelectedCard.cardButton.interactable = false;

    }

    /// <summary>多选逻辑
    /// </summary>
    /// <param name="selectedCard"></param>
    private void HandleMultiSelection(BackCardUI selectedCard)
    {
        if (multiSelectedCards.Contains(selectedCard))
        {
            // 取消选中
            multiSelectedCards.Remove(selectedCard);
            selectedCard.SetSelectedState(false);
            selectedCard.cardButton.interactable = true;
        }
        else
        {
            if (multiSelectedCards.Count >= maxMultiSelections)
            {
                var firstCard = multiSelectedCards[0];
                multiSelectedCards.RemoveAt(0);
                firstCard.SetSelectedState(false);
                firstCard.cardButton.interactable = true;
            }
            // 新增选中
            multiSelectedCards.Add(selectedCard);
            selectedCard.SetSelectedState(true);
            selectedCard.cardButton.interactable = false;
        }

    }

    /// <summary> 清除所有选中状态 </summary>
    public void ClearAllSelections()
    {
        // 清除单选状态
        if (singleSelectedCard != null)
        {
            singleSelectedCard.SetSelectedState(false);
            if (singleSelectedCard.cardButton != null)
                singleSelectedCard.cardButton.interactable = true;
            singleSelectedCard = null;
        }

        // 清除多选状态
        foreach (var card in multiSelectedCards)
        {
            if (card != null)
            {
                card.SetSelectedState(false);
                card.cardButton.interactable = true;
            }
        }
        multiSelectedCards.Clear();

        // 触发全局事件
        EventHandler.CallOnSelectionChanged(isSingleSelectionMode, singleSelectedCard, multiSelectedCards);
    }

    /// <summary>移除指定卡牌的选中状态
    /// </summary>
    /// <param name="card"></param>
    public void RemoveCardFromSelection(BackCardUI card)
    {
        //移除前先取消选中状态（隐藏描述框）
        card.SetSelectedState(false);

        if (singleSelectedCard == card)
            singleSelectedCard = null;

        if (multiSelectedCards.Contains(card))
            multiSelectedCards.Remove(card);

        // 触发全局事件
        EventHandler.CallOnSelectionChanged(isSingleSelectionMode, singleSelectedCard, multiSelectedCards);
    }


    //一些查询方法
    public bool IsSingleSelectionMode() => isSingleSelectionMode;
    public BackCardUI GetSingleSelectedCard() => singleSelectedCard;
    public List<BackCardUI> GetMultiSelectedCards() => new List<BackCardUI>(multiSelectedCards); //获取当前多选选中的卡牌列表(提供给出战列表使用)


}