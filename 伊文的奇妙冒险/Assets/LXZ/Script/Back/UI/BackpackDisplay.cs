using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackDisplay : MonoBehaviour
{
    private GameObject cardPrefab; //物品格子预制体
    private List<Transform> containers;//物品容器列表
    private int itemsPerContainer;//每个容器显示数量
    private SelectionManager selectionManager; //背包物品的选择和显示逻辑
    private List<GameObject> currentCards = new List<GameObject>(); //当前显示的物品UI列表

    // 初始化配置
    public void Initialize(GameObject prefab, List<Transform> containerList, int itemsPerSlot, SelectionManager selectionMgr)
    {
        cardPrefab = prefab;
        containers = containerList;
        itemsPerContainer = itemsPerSlot;
        selectionManager = selectionMgr;
    }

private void OnEnable()
    {
        // 订阅背包UI更新事件
        EventHandler.UpdateInventoryUI += OnInventoryUIUpdated;
    }

    private void OnDisable()
    {
        // 取消订阅，避免内存泄漏
        EventHandler.UpdateInventoryUI -= OnInventoryUIUpdated;
    }

/// <summary>通过事件接收数据并刷新UI</summary>
    private void OnInventoryUIUpdated(List<CardBasicInformation> dataList)
    {
        //ClearExistingCards();
        RefreshCards(dataList);
    }


    /// <summary>刷新 UI 背包显示
    /// </summary>
    /// <param name="dataList"></param>
    public void RefreshCards(List<CardBasicInformation> dataList)
    {
        ClearExistingCards();

        for (int i = 0; i < dataList.Count; i++)
        {
            CardBasicInformation data = dataList[i];
            if (data == null || data.cardId == 0) continue;

            int containerIndex = i / itemsPerContainer;
            if (containerIndex >= containers.Count)
            {

                continue;
            }

            GameObject newPrefab = Instantiate(cardPrefab, containers[containerIndex]);
            BackCardUI newCard = newPrefab.GetComponentInChildren<BackCardUI>();
            //更改 预设体的图片
            newCard.Initialize(data, selectionManager);

            currentCards.Add(newPrefab);
        }
    }

    // 清除现有卡牌UI
    public void ClearExistingCards()
    {
        foreach (var card in currentCards)
        {
            // 从选择管理器中移除该卡牌
            selectionManager.RemoveCardFromSelection(card.GetComponentInChildren<BackCardUI>());
            Destroy(card.gameObject);
        }
        currentCards.Clear();
    }
}