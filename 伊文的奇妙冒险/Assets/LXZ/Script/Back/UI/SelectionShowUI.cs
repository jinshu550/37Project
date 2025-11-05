using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionShowUI : MonoBehaviour
{
    [SerializeField] private GameObject showTheCardImage; // 右侧展示图
    [SerializeField] private TMP_Text showTheCarddescText; // 右侧展示图的描述
    [SerializeField] private TMP_Text showTheNameText; // 右侧展示图的描述
    [SerializeField] private GameObject syntheticPrefab;  //合成后生成的预设体
    [SerializeField] private Transform imagePos;  //放图片和预设体的位置
    private GameObject currentSynthesizedPrefab; // 缓存当前展示的合成预设体

    private void Awake()
    {
        // 初始化显示状态
        if (showTheCardImage != null)
        {
            showTheCardImage.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventHandler.OnSelectionChanged += OnGlobalSelectionChanged; // 订阅全局选择变化事件
        EventHandler.OnSynthesisResulted += OnSynthesisResultReceived; // 订阅合成结果事件（接收CardSynthesisManager的结果）
        EventHandler.OnSelectionCleared += ClearSynthesizedPrefab;// 订阅选择清除事件（切换物品时销毁预设体）
        EventHandler.OnDescriptionColorChanged += OnDescriptionColorChanged;// 订阅颜色变更事件
    }

    private void OnDisable()
    {
        // 取消订阅，避免内存泄漏
        EventHandler.OnSelectionChanged -= OnGlobalSelectionChanged;
        EventHandler.OnSynthesisResulted -= OnSynthesisResultReceived;
        EventHandler.OnSelectionCleared -= ClearSynthesizedPrefab;
        EventHandler.OnDescriptionColorChanged -= OnDescriptionColorChanged;
        ClearAllInstances();
    }

    // 处理颜色变更
    private void OnDescriptionColorChanged(Color color)
    {
        if (showTheCarddescText != null || showTheNameText != null)
        {
            showTheCarddescText.color = color;
            showTheNameText.color = color;
        }
    }

    /// <summary>响应全局选择状态变化事件</summary>
    private void OnGlobalSelectionChanged(bool isSingleMode, BackCardUI singleCard, List<BackCardUI> multiCards)
    {
        if (isSingleMode)
        {
            // 单选模式：显示图片物体，清除合成预设体
            UpdateSingleDisplay(singleCard);
            ClearSynthesizedPrefab();
        }
        else
        {
            // 多选模式：隐藏卡牌图 
            HideSingleDisplay();
        }
    }

    /// <summary>更新单选展示（显示图片物体）</summary>
    private void UpdateSingleDisplay(BackCardUI singleCard)
    {
        if (showTheCardImage == null) return;

        if (singleCard != null)
        {
            // 设置图片
            if (showTheCardImage.TryGetComponent<Image>(out Image image))
            {
                image.sprite = singleCard.cardDetails.battleSprite;
                image.preserveAspect = true;//保持图片原始的宽高比
            }
            showTheCardImage.SetActive(true);
            if (showTheCarddescText != null && singleCard.cardDetails != null)
            {
                showTheCarddescText.text = string.IsNullOrEmpty(singleCard.cardDetails.description)
                    ? "暂无描述信息"
                    : singleCard.cardDetails.description;

            }
            if (showTheNameText != null && singleCard.GetCardName() != null)
            {
                showTheNameText.text = string.IsNullOrEmpty(singleCard.GetCardName())
                    ? "暂无"
                    : singleCard.GetCardName();

            }
        }
        else
        {
            // 没有选中的卡牌时隐藏
            showTheCardImage.SetActive(false);
        }
    }

    /// <summary>接收合成结果：在指定位置显示预设体
    /// </summary>
    /// <param name="targetCard">目标卡牌</param>
    private void OnSynthesisResultReceived(CardBasicInformation targetCard)
    {
        // 先销毁之前的预设体
        ClearSynthesizedPrefab();

        //生成 合成预设体 并修改预设体的图片
        GameObject prefab = Instantiate(syntheticPrefab, imagePos);
        if (prefab == null)
            return;

        prefab.name = "展示合成卡牌";
        prefab.tag = "Card";
        currentSynthesizedPrefab = prefab;

        prefab.GetComponent<SynthesizedCard>().Init(targetCard);

        SynthesizedCard synthesizedCard = prefab.GetComponent<SynthesizedCard>();
        // synthesizedCard.targetCard = targetCard; // 赋值合成目标卡牌
        // synthesizedCard.cardID = targetCard.cardId; // 赋值合成目标卡牌的ID

        //绑定点击事件，响应UGUI点击
        EventTrigger eventTrigger = prefab.GetComponent<EventTrigger>();
        eventTrigger.triggers.Clear();// 清除原有事件（避免重复绑定）

        // 创建点击事件条目
        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        // 绑定点击回调（调用ItemPickUp的收集UI卡牌方法）
        clickEntry.callback.AddListener((data) =>
        {
            // 找到场景中的ItemPickUp实例，执行收集
            ItemPickUp itemPickUp = FindObjectOfType<ItemPickUp>();
            if (itemPickUp != null)
            {
                itemPickUp.CollectUISynthesizedCard(synthesizedCard);
            }
        });
        // 添加事件到EventTrigger
        eventTrigger.triggers.Add(clickEntry);


        // 确保合成预设体处于激活状态
        if (currentSynthesizedPrefab == null) return;
        else
            currentSynthesizedPrefab.SetActive(true);
    }

    /// <summary>隐藏单选展示</summary>
    private void HideSingleDisplay()
    {
        if (showTheCardImage != null)
        {
            showTheCardImage.SetActive(false);
        }
    }

    /// <summary> 销毁当前合成的预设体 </summary>
    private void ClearSynthesizedPrefab()
    {
        if (currentSynthesizedPrefab != null)
        {
            Destroy(currentSynthesizedPrefab);
            currentSynthesizedPrefab = null;
        }
    }

    /// <summary>清除所有展示实例（包括图片和预设体）</summary>
    private void ClearAllInstances()
    {
        HideSingleDisplay();
        ClearSynthesizedPrefab();
    }

}
