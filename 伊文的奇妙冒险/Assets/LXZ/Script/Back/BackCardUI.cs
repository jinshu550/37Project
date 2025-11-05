using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BackCardUI : MonoBehaviour, IPointerClickHandler
{
    [Header("基础组件")]
    private string cardName;
    [SerializeField] private Image backCardImage; //卡牌图片
    [SerializeField] public Button cardButton; // 引用自身按钮组件
    [SerializeField] public CardBasicInformation cardDetails;  //卡牌数据

    [Header("出战状态")]
    [SerializeField] private Image battleIcon; // 出战图标

    [Header("描述框配置")]
    //[SerializeField] private Image descPanel;    // 描述框面板
    [SerializeField] private TMP_Text descText;  // 描述文本
    [SerializeField] private TMP_Text nameText;  // 卡牌名字文本

    private SelectionManager selectionManager; // 选择控制
    private bool isSelected = false;//本地记录是否选中（高亮）
    private bool isInBattle = false; // 记录是否在出战列表中


    private void Awake()
    {
        // 自动获取按钮组件（如果未手动赋值）
        if (cardButton == null)
            cardButton = GetComponent<Button>();

        // 确保按钮可交互（否则状态动画不生效）
        if (cardButton != null)
            cardButton.interactable = true;

    }

    // 初始化卡牌数据和依赖
    public void Initialize(CardBasicInformation data, SelectionManager selectionMgr)
    {
        cardDetails = data;
        cardName = data.cardName;
        selectionManager = selectionMgr;
        backCardImage.sprite = data.battleSprite; // 卡牌图片赋值
        backCardImage.preserveAspect = true;

        // 描述框文本预赋值（避免显示空文本）
        if (descText != null && cardDetails != null)
        {
            descText.text = string.IsNullOrEmpty(cardDetails.description)
                ? "暂无描述信息"
                : cardDetails.description;
        }
        // 名字文本预赋值（避免显示空文本）
        if (nameText != null && cardDetails != null)
        {
            nameText.text = string.IsNullOrEmpty(cardName)
                ? "暂无描述信息"
                : cardName;
        }
    }

    public string GetCardName()
    {
        return cardName;
    }

    /// <summary>处理卡牌点击事件
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 点击时通知 selectionManager 处理选中的事件
        if (selectionManager != null && cardDetails != null)
        {
            selectionManager.OnCardSelected(this);
        }
    }


    /// <summary>选中按钮后的事件
    /// </summary>
    /// <param name="isSelected"></param>
    public void SetSelectedState(bool newState)
    {
        if (cardButton == null) return;

        isSelected = newState;
        //选中时显示描述框，取消选中时隐藏
        // if (isSelected)
        //     ShowDescriptionPanel();
        // else
        //     HideDescriptionPanel();

        // 检查 EventSystem 是否存在且未被销毁
        if (EventSystem.current != null && !EventSystem.current.IsDestroyed())
        {
            EventSystem.current.SetSelectedGameObject(isSelected ? gameObject : null);
        }

    }

    /// <summary>
    /// 切换出战状态
    /// </summary>
    /// <param name="inBattle"></param>
    public void SetBattleState(bool inBattle)
    {
        isInBattle = inBattle;
        if (battleIcon != null)
        {
            battleIcon.gameObject.SetActive(isInBattle);  // 出战图标显示 / 隐藏
        }
    }

}
