using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemGetWindow : MonoBehaviour
{
    [Header("物品获取面板")]
    [SerializeField] private GameObject itemGetWindow;
    [SerializeField] private TMP_Text popupTitle; // 弹窗标题文本
    [SerializeField] private TMP_Text popupContent; // 弹窗内容文本
    [SerializeField] private Button itemGetBtn;  //弹窗按钮



    void OnEnable()
    {
        EventHandler.OnItemCollected += OnItemCollected;
        EventHandler.OnLackItem += OnLackItem;
    }

    void OnDisable()
    {
        EventHandler.OnItemCollected -= OnItemCollected;
        EventHandler.OnLackItem -= OnLackItem;
    }

    private void OnItemCollected(string itemName)
    {
        if (itemGetWindow != null && itemGetBtn != null)
        {
            itemGetWindow.SetActive(true);
            EventHandler.CallOnDialogueStateChanged(true);
            popupTitle.text = $"获得物品";
            popupContent.text = $"你获得了物品：{itemName}";
            // 绑定确认按钮点击事件
            if (itemGetBtn != null)
                itemGetBtn.onClick.AddListener(OnConfirmBtnClicked);
        }
    }

    private void OnLackItem(int itemID)
    {
        if (itemGetWindow != null && itemGetBtn != null)
        {
            itemGetWindow.SetActive(true);
            EventHandler.CallOnDialogueStateChanged(true);
            switch (itemID)
            {
                case 5006:
                    popupTitle.text = $"缺少物品";
                    popupContent.text = $"你并没有酒可以完成承诺，记住配方是夜露水、月光草和黄金果。请收集后在背包中合成它。";
                    break;
                
                default:
                    break;
            }
            // 绑定确认按钮点击事件
            if (itemGetBtn != null)
                itemGetBtn.onClick.AddListener(OnConfirmBtnClicked);
        }
    }


    private void OnConfirmBtnClicked()
    {
        // 隐藏弹窗，重置弹窗状态
        if (itemGetWindow != null)
            itemGetWindow.SetActive(false);

        EventHandler.CallOnDialogueStateChanged(false);
    }


}

