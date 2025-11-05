using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//拓展状态效果数据结构，记录持续回合信息
public class TimedStatusEffect
{
    public StatusEffectType EffectType;
    public int Stacks;
    public int EndTurn;//效果结束的回合
}
public class StatusEffectSystem : Singleton<StatusEffectSystem>
{
    [SerializeField] private List<StatusEffectType> oneTurnEffects = new();
    [SerializeField] Dictionary<CombatantView, List<TimedStatusEffect>> timedEffects = new();
    [SerializeField] private GameObject ShieldPrefab;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<AddStatusEffectGA>(AddStatusEffectPerformer);
        ActionSystem.SubscribeReaction<TurnCounterGA>(CheckAndRemoveTimedEffects, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<AddStatusEffectGA>();
        ActionSystem.UnsubscribeReaction<TurnCounterGA>(CheckAndRemoveTimedEffects, ReactionTiming.POST);
    }
    private IEnumerator AddStatusEffectPerformer(AddStatusEffectGA addStatusEffectGA)
    {
        if (addStatusEffectGA.Target != null)
        {
            var single = addStatusEffectGA.Target;
            //添加效果到目标
            single.AddStatusEffect(addStatusEffectGA.StatusEffectType, addStatusEffectGA.StackCount);
            //计算效果回合数
            int endTurn = CardSystem.Instance.TurnCounter + addStatusEffectGA.Duration;
            //初始化目标的效果列表
            if (!timedEffects.ContainsKey(single))
            {
                timedEffects[single] = new List<TimedStatusEffect>();
            }
            timedEffects[single].Add(new TimedStatusEffect
            {
                EffectType = addStatusEffectGA.StatusEffectType,
                Stacks = addStatusEffectGA.StackCount,
                EndTurn = endTurn
            });
        }
        else
        {
            foreach (var target in addStatusEffectGA.Targets)
            {
                //添加效果到目标
                target.AddStatusEffect(addStatusEffectGA.StatusEffectType, addStatusEffectGA.StackCount);
                if (addStatusEffectGA.StatusEffectType == StatusEffectType.ARMOR && ShieldPrefab != null)
                {
                    GameObject shield = Instantiate(ShieldPrefab, target.transform.position, Quaternion.identity);
                    shield.transform.SetParent(target.transform);
                    Destroy(shield, 2f);
                }
                //计算效果回合数
                int endTurn = CardSystem.Instance.TurnCounter + addStatusEffectGA.Duration;
                //初始化目标的效果列表
                if (!timedEffects.ContainsKey(target))
                {
                    timedEffects[target] = new List<TimedStatusEffect>();
                }
                timedEffects[target].Add(new TimedStatusEffect
                {
                    EffectType = addStatusEffectGA.StatusEffectType,
                    Stacks = addStatusEffectGA.StackCount,
                    EndTurn = endTurn
                });
                yield return null;
            }
        }
    }

    private void CheckAndRemoveTimedEffects(TurnCounterGA turnCounterGA)
    {
        // 获取当前回合数
        int currentTurn = CardSystem.Instance.TurnCounter;

        // 遍历所有战斗单位的持续效果
        foreach (var combatant in new List<CombatantView>(timedEffects.Keys))
        {
            // 找出所有已到期的效果
            var expiredEffects = timedEffects[combatant]
                .Where(e => e.EndTurn <= currentTurn)
                .ToList();

            // 移除已到期的效果
            foreach (var effect in expiredEffects)
            {
                combatant.RemoveStatusEffect(effect.EffectType, effect.Stacks);
                timedEffects[combatant].Remove(effect);
            }

            // 清理空列表
            if (timedEffects[combatant].Count == 0)
            {
                timedEffects.Remove(combatant);
            }
        }
    }
    //敌人回合后移除状态
}
