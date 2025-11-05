using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewHoverSystem : Singleton<CardViewHoverSystem>
{
    [SerializeField] private CardView cardView;
    public void Show(Card card, Vector3 position)
    {
        cardView.gameObject.SetActive(true);
        cardView.SetUp(card);
        cardView.transform.position = position;
    }
    public void Hide()
    {
        cardView.gameObject.SetActive(false);
    }
}
