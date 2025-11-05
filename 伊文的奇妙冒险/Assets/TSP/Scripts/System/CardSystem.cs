using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPiplePoint;
    [SerializeField] private Transform discardPiplePoint;
    [SerializeField] private int baseDrawCard;                      //基础手牌数
    [SerializeField] private TextMeshPro TurnCountsText;
    [SerializeField] private Animator animator;
    private int maxDrawCard;
    public bool IsEnemyHandDisabled { get; private set; }           //是否禁手
    public bool IsDiscarding { get; private set; }                  //是否处于弃牌阶段
    public int RequiredDiscardCount { get; private set; }           //需要弃牌的数量
    public List<Card> selectedForDiscard = new();                  //选中要弃置的卡牌
    private readonly List<Card> drawPile = new();                   //抽牌
    private readonly List<Card> discardPile = new();                //弃牌堆
    private readonly List<Card> hand = new();                       //手牌
    public readonly List<Card> functionCards = new();              //功能牌
    public int TurnCounter { get; private set; }                    //记录回合数量
    private bool isWaitingForDiscardConfirmation;                   //新增：是否在等待弃牌确认
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardsGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllGA>(DiscardAllCardsPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
        ActionSystem.AttachPerformer<ReturnToHandGA>(ReturnToHandPerformer);
        ActionSystem.AttachPerformer<TurnCounterGA>(RecordTurnCountsPerformer);
        ActionSystem.AttachPerformer<DisableEnemyHandGA>(DisableEnemyHandPerformer);
        ActionSystem.AttachPerformer<DiscardGA>(BeginDiscardingPhase);
        ActionSystem.AttachPerformer<MaxHandCardsGA>(MaxHandsPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(TurnCounterReaction, ReactionTiming.PRE);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardsGA>();
        ActionSystem.DetachPerformer<DiscardAllGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
        ActionSystem.DetachPerformer<ReturnToHandGA>();
        ActionSystem.DetachPerformer<TurnCounterGA>();
        ActionSystem.DetachPerformer<DisableEnemyHandGA>();
        ActionSystem.DetachPerformer<DiscardGA>();
        ActionSystem.DetachPerformer<MaxHandCardsGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(TurnCounterReaction, ReactionTiming.PRE);
    }

    //Public
    public void SetUp(List<CardData> deckData, List<CardData> buffData)
    {
        TurnCounter = 1;
        UpdateTurnCounts();
        maxDrawCard = baseDrawCard;
        //初始化牌库
        // 抽牌堆
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
        //功能牌堆 并且 执行效果
        if (buffData != null)
        {
            foreach (var cardData in buffData)
            {
                // 创建功能牌实例
                Card functionCard = new Card(cardData);
                // 这里可以将功能牌添加到特定的列表中保存
                functionCards.Add(functionCard);
            }
            if (FunctionCardsUI.Instance != null)
            {
                FunctionCardsUI.Instance.InitFunctionCardsUI();
            }
        }

    }
    //禁用手牌
    private IEnumerator DisableEnemyHandPerformer(DisableEnemyHandGA disableEnemyHandGA)
    {
        //标记敌方手牌在下一回合
        IsEnemyHandDisabled = true;
        yield return null;
    }
    //记录回合数
    private IEnumerator RecordTurnCountsPerformer(TurnCounterGA turnCounterGA)
    {
        TurnCounter += 1;
        UpdateTurnCounts();
        yield return new WaitForSeconds(0.1f);
    }


    //Performs
    //执行功能牌效果
    private IEnumerator SetUpFunctionPerformer()
    {
        //遍历并执行效果
        foreach (var card in functionCards)
        {
            foreach (var effectWrapper in card.OtherEffects)
            {
                List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
                PerformEffectGA performEffectGA = new(effectWrapper.Effect, targets);
                // 立即执行效果或添加到反应队列
                ActionSystem.Instance.AddReaction(performEffectGA);
            }
            yield return null;
        }
    }
    private IEnumerator DrawCardsPerformer(DrawCardsGA drawCardsGA)
    {
        int actualAmount = Mathf.Min(drawCardsGA.Amount, drawPile.Count);
        int notDrawnAmount = drawCardsGA.Amount - actualAmount;
        for (int i = 0; i < actualAmount; i++)
        {
            yield return DrawCards();
        }
        if (notDrawnAmount > 0)
        {
            RefillDeck();
            for (int i = 0; i < notDrawnAmount; i++)
            {
                yield return DrawCards();
            }
        }
        if (TurnCounter == 1)
        {
            yield return SetUpFunctionPerformer();
        }
    }
    private IEnumerator DiscardAllCardsPerformer(DiscardAllGA discardAllGA)
    {
        foreach (var card in hand)
        {
            CardView cardView = handView.RemoveCard(card);
            yield return DiscardCard(cardView);
        }
        hand.Clear();
    }
    // 新增：处理弃牌逻辑
    private IEnumerator ProcessDiscard()
    {
        // 执行弃牌
        foreach (var card in selectedForDiscard)
        {
            CardView cardView = handView.RemoveCard(card);
            yield return DiscardCard(cardView);
            hand.Remove(card);
        }

        // 结束弃牌阶段
        IsDiscarding = false;
        isWaitingForDiscardConfirmation = false;
        selectedForDiscard.Clear();
        RequiredDiscardCount = 0;
    }
    private IEnumerator BeginDiscardingPhase(DiscardGA discardGA)
    {
        IsDiscarding = true;
        selectedForDiscard.Clear();
        isWaitingForDiscardConfirmation = true;
        //通知玩家需要弃牌 UI
        DiscardButtonUI.Instance.HintDiscount(selectedForDiscard.Count, RequiredDiscardCount);
        //等待弃牌执行完成
        while (isWaitingForDiscardConfirmation)
        {
            yield return null;
        }
        IsDiscarding = false;
    }
    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        // 记录当前打出的卡牌
        Card.CurrentPlayingCard = playCardGA.Card;
        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);
        yield return DiscardCard(cardView);

        SpendManaGA spendManaGA = new(playCardGA.Card.Mana);
        ActionSystem.Instance.AddReaction(spendManaGA);

        //effects
        if (playCardGA.Card.ManualTargetEffect != null)
        {
            PerformEffectGA performEffectGA = new(playCardGA.Card.ManualTargetEffect, new() { playCardGA.ManualTarget });
            ActionSystem.Instance.AddReaction(performEffectGA);
            animator.SetTrigger("Attack");
            StartCoroutine(BackToIdle());
        }
        foreach (var effectWrapper in playCardGA.Card.OtherEffects)
        {
            List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
            PerformEffectGA performEffectGA = new(effectWrapper.Effect, targets);
            ActionSystem.Instance.AddReaction(performEffectGA);
        }
    }
    private IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("BackToIdle");
    }
    private IEnumerator MaxHandsPerformer(MaxHandCardsGA maxHandCardsGA)
    {
        maxDrawCard = baseDrawCard + maxHandCardsGA.Amount;
        yield return null;
    }
    public bool HasRequiredCard(String title)
    {
        foreach (var card in functionCards)
        {
            if (card.Title == title)
            {
                return true;
            }
        }
        return false;
    }
    //Reactions
    private void EnemyTurnPreReaction(EnemyTurnGA enemyTurnGA)
    {
        if (IsEnemyHandDisabled)
        {
            IsEnemyHandDisabled = false;
            return;
        }
        if (hand.Count > maxDrawCard)
        {
            RequiredDiscardCount = hand.Count - maxDrawCard;
            DiscardGA discardGA = new DiscardGA();
            ActionSystem.Instance.AddReaction(discardGA);
        }
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        if (IsEnemyHandDisabled)
        {
            return;
        }
        DrawCardsGA drawCardsGA = new(2);
        ActionSystem.Instance.AddReaction(drawCardsGA);
    }
    private void TurnCounterReaction(EnemyTurnGA enemyTurnGA)
    {
        TurnCounterGA turnCounterGA = new();
        ActionSystem.Instance.AddReaction(turnCounterGA);
    }
    private void UpdateTurnCounts()
    {
        TurnCountsText.text = "回合:" + " " + TurnCounter;
    }
    public void ExecuteDiscard()
    {
        if (!IsDiscarding || !isWaitingForDiscardConfirmation) return;

        StartCoroutine(ProcessDiscard());
    }
    //Help Function
    public bool ToggleCardForDiscard(Card card)
    {
        if (!IsDiscarding) return false; // 不在弃牌阶段，返回false

        if (selectedForDiscard.Contains(card))
        {
            // 取消选中
            selectedForDiscard.Remove(card);
            DiscardButtonUI.Instance.HintDiscount(selectedForDiscard.Count, RequiredDiscardCount);
            return false; // 已取消选中，返回false
        }
        else
        {
            // 选中弃牌
            if (selectedForDiscard.Count < RequiredDiscardCount)
            {
                selectedForDiscard.Add(card);
                DiscardButtonUI.Instance.HintDiscount(selectedForDiscard.Count, RequiredDiscardCount);
                return true; // 已选中，返回true
            }
        }
        // 无法选中（已达最大数量），返回false
        return false;
    }
    private IEnumerator DrawCards()
    {
        Card card = drawPile.Draw();
        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPiplePoint.position, drawPiplePoint.rotation);
        yield return handView.AddCard(cardView);
    }
    private void RefillDeck()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
    }
    private IEnumerator DiscardCard(CardView cardView)
    {
        discardPile.Add(cardView.Card);
        cardView.transform.DOScale(Vector3.zero, 0.15f);
        Tween tween = cardView.transform.DOMove(discardPiplePoint.position, 0.15f);
        yield return tween.WaitForCompletion();
        Destroy(cardView.gameObject);
    }
    private IEnumerator ReturnToHandPerformer(ReturnToHandGA returnToHandGA)
    {
        Card card = returnToHandGA.CardToHand;
        discardPile.Remove(card);
        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPiplePoint.position, drawPiplePoint.rotation);
        yield return handView.AddCard(cardView);
    }


}
