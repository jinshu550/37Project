using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;


public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    private Animator animator;
    public List<EnemyView> Enemies => enemyBoardView.EnemyViews;
    private List<CardData> playCard = new();
    private List<EnemyPlannedCard> nextTurnEnemyPlans = new List<EnemyPlannedCard>();
    private List<CardData> buffCards = new();
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<KillEnemyGA>(KillEnemyPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(ShowPrefabActionReaction, ReactionTiming.POST);

    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<KillEnemyGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(ShowPrefabActionReaction, ReactionTiming.POST);
    }
    private void GenerateNextTurnPlans()
    {
        nextTurnEnemyPlans.Clear();
        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            if (enemy.CurrentHealth <= 0) continue;

            var enemyData = enemy.GetEnemyData();
            //跳过没有牌的敌人
            if (enemyData.Deck.Count == 0) continue;
            //选中一张牌
            int randomIndex = Random.Range(0, enemyData.Deck.Count);
            CardData data = enemyData.Deck[randomIndex];
            //把卡牌放进计划里
            nextTurnEnemyPlans.Add(new EnemyPlannedCard(enemy, new List<CardData> { data }));

        }
    }
    private void ShowPrefabActionReaction(EnemyTurnGA enemyTurnGA)
    {
        GenerateNextTurnPlans();
        PreviewActionUI.Instance.UpdateActionPreviews(nextTurnEnemyPlans);
    }
    public void SetUp(List<EnemyData> enemyDatas)
    {
        foreach (var enemyData in enemyDatas)
        {
            enemyBoardView.AddEnemy(enemyData);
            GenerateNextTurnPlans();
            PreviewActionUI.Instance.UpdateActionPreviews(nextTurnEnemyPlans);
        }
        foreach (var enemyView in Enemies)
        {
            if (StaticUtility.CurrentEnemyBuff != null)
            {
                if (StaticUtility.CurrentEnemyBuff.Name == "Angry")
                {
                    enemyView.AddStatusEffect(StatusEffectType.Angry, 1);
                }
                else if (StaticUtility.CurrentEnemyBuff.Name == "Fulfill")
                {
                    enemyView.AddStatusEffect(StatusEffectType.Fulfill, 1);
                }
            }
            else
            {
                continue;
            }

        }
    }

    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurnGA)
    {

        playCard.Clear();
        //检测是否被禁用手牌
        if (CardSystem.Instance.IsEnemyHandDisabled)
        {
            GenerateNextTurnPlans();
            PreviewActionUI.Instance.UpdateActionPreviews(nextTurnEnemyPlans);
            yield break;
        }
        // 执行「当前回合计划」
        foreach (var plan in nextTurnEnemyPlans)
        {
            EnemyView enemy = plan.Enemy;
            // 跳过已死亡或没卡的敌人
            if (enemy.CurrentHealth <= 0 || plan.PlannedCards.Count == 0) continue;

            if (enemy.FunctionCards != null)
            {
                List<CardData> cardDatas = enemy.FunctionCards;
                buffCards = cardDatas;
                if (StaticUtility.CurrentEnemyBuff != null && !buffCards.Contains(StaticUtility.CurrentEnemyBuff))
                {
                    buffCards.Add(StaticUtility.CurrentEnemyBuff);
                }
                for (int i = 0; i < buffCards.Count; i++)
                {
                    CardData card = buffCards[i];
                    for (int j = 0; j < card.OtherEffects.Count; j++)
                    {
                        // 执行效果
                        AutoTargetEffect effectWrapper = card.OtherEffects[j];
                        List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
                        PerformEffectGA performEffectGA = new PerformEffectGA(effectWrapper.Effect, targets);
                        ActionSystem.Instance.AddReaction(performEffectGA);
                    }
                }
            }
            animator = enemy.Animator;
            CardData currentCard = plan.PlannedCards[0]; // 取这回合要出的1张卡
            playCard.Add(currentCard);

            // 执行这张卡的所有效果
            foreach (var effectWrapper in currentCard.OtherEffects)
            {
                // 攻击动画逻辑
                if (effectWrapper.Effect is DealDamageEffect)
                {
                    RectTransform rectTransform = enemy.imageTransform;
                    Vector2 initialAnchoredPosition = rectTransform.anchoredPosition;
                    Tween tween = rectTransform.DOAnchorPosX(initialAnchoredPosition.x - 100f, 0.25f);
                    yield return tween.WaitForCompletion();
                    rectTransform.DOAnchorPosX(initialAnchoredPosition.x, 0.15f);
                }
                // 执行效果
                List<CombatantView> targets = effectWrapper.TargetMode.GetTargets();
                PerformEffectGA performEffectGA = new PerformEffectGA(effectWrapper.Effect, targets);
                ActionSystem.Instance.AddReaction(performEffectGA);
            }
        }
    }

    private IEnumerator KillEnemyPerformer(KillEnemyGA killEnemyGA)
    {
        yield return enemyBoardView.RemoveEnemy(killEnemyGA.EnemyView);
    }

    private IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("BackToIdle");
    }
}