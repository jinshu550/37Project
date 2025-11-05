using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynthesizedCard : MonoBehaviour
{
    public int cardID; // 合成卡牌的唯一ID
    [SerializeField] private Image showImage;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text nameText;

    public CardBasicInformation targetCard;//（由CardSynthesisManager赋值）

    void OnEnable()
    {
        
        EventHandler.OnDescriptionColorChanged += OnDescriptionColorChanged;// 订阅颜色变更事件
    }

    void OnDisable()
    {
        
        EventHandler.OnDescriptionColorChanged -= OnDescriptionColorChanged;// 订阅颜色变更事件
    }

    private void OnDescriptionColorChanged(Color color)
    {
        if (descText != null && nameText != null)
        {
            descText.color = color;
            nameText.color = color; 
        }
    }

    public void Init(CardBasicInformation target)
    {
        if (target == null)
        {
            Debug.LogError("SynthesizedCard: targetCard为空！");
            return;
        }
        targetCard = target;
        cardID = target.cardId;
        showImage.sprite = target.battleSprite;
        showImage.preserveAspect = true;
        showImage.raycastTarget = true;
        if (descText != null && target != null)
        {
            descText.text = string.IsNullOrEmpty(target.description)
                ? "暂无描述信息"
                : target.description;
        }
        if (nameText != null && target != null)
        {
            nameText.text = string.IsNullOrEmpty(target.cardName)
                ? "暂无描述信息"
                : target.cardName;
        }
    }
}