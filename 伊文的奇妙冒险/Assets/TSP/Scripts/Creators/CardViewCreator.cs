using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;
    public CardView CreateCardView(Card card, Vector3 position, Quaternion rotation)
    {
        CardView cardView = Instantiate(cardViewPrefab, position, rotation);
        cardView.transform.localScale = Vector3.zero;
        cardView.transform.DOScale(new Vector3(0.5f, 0.5f, 1), 0.15f);
        cardView.SetUp(card);
        return cardView;
    }
}
