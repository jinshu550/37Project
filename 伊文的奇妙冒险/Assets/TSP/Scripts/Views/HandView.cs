using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using System.Linq;

public class HandView : MonoBehaviour
{
    [SerializeField] private int maxHandSize;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    private readonly List<CardView> handCards = new List<CardView>();


    public IEnumerator AddCard(CardView cardView)
    {
        if (handCards.Count >= maxHandSize) yield break;
        handCards.Add(cardView);
        yield return UpdateCardPosition(0.15f);

    }
    public CardView RemoveCard(Card card)
    {
        CardView cardView = GetCardView(card);
        if (cardView == null) return null;
        handCards.Remove(cardView);
        StartCoroutine(UpdateCardPosition(0.15f));
        return cardView;
    }
    private CardView GetCardView(Card card)
    {
        return handCards.Where(CardView => CardView.Card == card).FirstOrDefault();
    }
    private IEnumerator UpdateCardPosition(float duration)
    {
        if (handCards.Count == 0) yield break;
        float cardSpacing = 1f / maxHandSize;
        float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;
        Spline spline = splineContainer.Spline;
        for (int i = 0; i < handCards.Count; i++)
        {
            float p = firstCardPosition + i * cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            handCards[i].transform.DOMove(splinePosition + transform.position + 0.01f * i * Vector3.back, duration);
            handCards[i].transform.DORotate(rotation.eulerAngles, duration);
        }
        yield return new WaitForSeconds(duration);
    }
}
