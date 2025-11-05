using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PreviewActionUI : Singleton<PreviewActionUI>
{
    private Transform actionContainer;
    [SerializeField] private GameObject previewPrefab;
    [Header("cardType-sprite")]
    public Sprite attackIcon;
    public Sprite healthIcon;
    public Sprite defendIcon;

    public void UpdateActionPreviews(List<EnemyPlannedCard> enemyPlanneds)
    {

        ClearPreviews();
        foreach (var plan in enemyPlanneds)
        {
            actionContainer = transform;
            //取第一张牌
            var planCard = plan.PlannedCards[0];
            GameObject previewObj = Instantiate(previewPrefab, actionContainer, false);
            previewObj.transform.localScale = Vector3.one;
            //1.找到预制体里的Image和Text
            Image actionIcon = previewObj.GetComponent<Image>();
            //2.
            switch (planCard.CardType)
            {
                case CardType.Attack:
                    actionIcon.sprite = attackIcon;
                    break;
                case CardType.Defend:
                    actionIcon.sprite = defendIcon;
                    break;
                case CardType.Health:
                    actionIcon.sprite = healthIcon;
                    break;
            }
        }
    }
    //清空预告
    public void ClearPreviews()
    {
        if (actionContainer == null) return;
        foreach (Transform child in actionContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
