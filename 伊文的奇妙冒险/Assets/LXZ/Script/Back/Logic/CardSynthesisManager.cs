using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardSynthesisManager : MonoBehaviour
{
    private void OnEnable()
    {
        // 订阅合成请求事件（接收BackpackUIManager的请求）
        EventHandler.OnSynthesisRequested += HandleSynthesisRequest;
    }

    private void OnDisable()
    {
        // 取消订阅
        EventHandler.OnSynthesisRequested -= HandleSynthesisRequest;
    }

    /// <summary> 检查选中的卡牌可以合成哪些卡牌
    /// </summary>
    /// <param name="selectedCards">选中的卡牌列表</param>
    /// <returns>可合成的卡牌列表</returns>
    public void HandleSynthesisRequest(List<BackCardUI> selectedCards)
    {

        EventHandler.CallOnSelectionCleared();  //先销毁之前的合成预设体（避免重复）

        CardBasicDatabase cardDatabase = InventoryManager.Instance.allFunctionCardDB;  //获取数据库

        //提取选中卡牌ID （去重但保留数量，适配2/3合1）
        List<int> selectedIds = selectedCards
            .Where(card => card != null && card.cardDetails != null)
            .Select(card => card.cardDetails.cardId)
            .ToList();
        if (selectedIds.Count < 2) return;  //如果选的少于2则返回空  不进行一个合成查询

        //查找可合成的目标卡牌（匹配配方ID和数量）
        CardBasicInformation targetCard = FindSynthesizableCard(selectedIds, cardDatabase);

        if (targetCard == null)
        {

            if (SelectionManager.Instance != null)
            {
                SelectionManager.Instance.ClearAllSelections();
            }
            else
            {
            }

            // 触发合成失败事件（通知BackpackUIManager抖动按钮）
            EventHandler.CallOnSynthesisFailed();

            return;
        }

        //触发合成结果事件（通知SelectionShowUI展示）
        EventHandler.CallOnSynthesisResulted(targetCard);

    }

    /// <summary>查找可合成的目标卡牌（匹配ID和数量）
    /// </summary>
    /// <param name="selectedIds">传入选中的卡牌</param>
    /// <param name="cardDB">数据库</param>
    /// <returns></returns>
    private CardBasicInformation FindSynthesizableCard(List<int> selectedIds, CardBasicDatabase cardDB)
    {
        foreach (var candidate in cardDB.allExtendedCards)
        {
            if (candidate == null || candidate.requiredCardIds == null || candidate.requiredCardIds.Count == 0)
                continue;

            // 条件1：合成所需数量 = 选中数量（2合1/3合1）
            if (candidate.requiredCardIds.Count != selectedIds.Count)
                continue;

            // 条件2：排序后ID完全匹配（避免顺序影响）
            List<int> sortedRequired = new List<int>(candidate.requiredCardIds); //查找的合成配方
            List<int> sortedSelected = new List<int>(selectedIds);   //传入的选中卡牌
            sortedRequired.Sort();
            sortedSelected.Sort();

            if (sortedRequired.SequenceEqual(sortedSelected))
            {
                return candidate; // 找到匹配配方
            }
        }
        return null;
    }


}
