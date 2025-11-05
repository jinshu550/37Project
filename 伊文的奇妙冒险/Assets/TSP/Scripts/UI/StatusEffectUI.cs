using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text stackCountText;
    public void Set(Sprite sprite, int stackCount, bool hideStack)
    {
        image.sprite = sprite;
        if (!hideStack)
        {
            stackCountText.text = stackCount.ToString();
        }
        else
        {
            stackCountText.gameObject.SetActive(false);
        }


    }
    public void AdjustTextSize()
    {
        TextMeshProUGUI text = transform.GetComponentInChildren<TextMeshProUGUI>();
        text.fontSize = 100;
        text.rectTransform.anchoredPosition = new Vector2(100, 0);
    }

}
