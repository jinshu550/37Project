using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageSystem : Singleton<DamageSystem>
{
    [SerializeField] private GameObject damageVFX;
    [SerializeField] private SettlementUI settlementUI;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
        ActionSystem.AttachPerformer<Armor_PiercingGA>(Armor_PiercingPerformer);
        ActionSystem.AttachPerformer<IncreaseDamageGA>(DamageAfterTurnPerformer);       //结算伤害
        ActionSystem.SubscribeReaction<EnemyTurnGA>(DamageOfTurnPostReaction, ReactionTiming.PRE);

    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
        ActionSystem.DetachPerformer<Armor_PiercingGA>();
        ActionSystem.DetachPerformer<IncreaseDamageGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(DamageOfTurnPostReaction, ReactionTiming.PRE);
    }
    private IEnumerator DealDamagePerformer(DealDamageGA dealDamageGA)
    {
        foreach (var target in dealDamageGA.Targets)
        {
            target.Damage(dealDamageGA.Amount);
            //Instantiate(damageVFX, target.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            if (target.CurrentHealth <= 0)
            {
                if (target is EnemyView enemyView)
                {
                    KillEnemyGA killEnemyGA = new KillEnemyGA(enemyView);
                    ActionSystem.Instance.AddReaction(killEnemyGA);
                    settlementUI.MoveSettlementUIDown(true);
                }
                else
                {
                    //Do some game over logic here
                    //Open Game Over Scene
                    settlementUI.MoveSettlementUIDown(false);
                }
            }
        }
    }
    private IEnumerator Armor_PiercingPerformer(Armor_PiercingGA armor_PiercingGA)
    {
        foreach (var target in armor_PiercingGA.Targets)
        {
            target.HalveArmor();
        }
        yield return new WaitForSeconds(0.1f);
    }
    private IEnumerator DamageAfterTurnPerformer(IncreaseDamageGA increaseDamageGA)
    {
        foreach (var target in increaseDamageGA.Targets)
        {
            target.Damage(increaseDamageGA.Amount);
            //Instantiate(damageVFX, target.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            if (target.CurrentHealth <= 0)
            {
                if (target is EnemyView enemyView)
                {
                    KillEnemyGA killEnemyGA = new KillEnemyGA(enemyView);
                    ActionSystem.Instance.AddReaction(killEnemyGA);
                }
                else
                {
                    //Do some game over logic here

                    //Open Game Over Scene

                }
            }
        }
    }
    //结算伤害
    private void DamageOfTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        if (!CardSystem.Instance.HasRequiredCard("ElfArrow") || !CardSystem.Instance.HasRequiredCard("GoldenSpecialBlend")
         || StaticUtility.CurrentEnemyBuff.ToString() != "Angry") return;
        List<CombatantView> targets = new List<CombatantView>(EnemySystem.Instance.Enemies);
        // 创建伤害行动并添加到行动系统执行
        IncreaseDamageGA increaseDamageGA = new IncreaseDamageGA(
            targets,
            1
        );
        // 执行伤害逻辑
        ActionSystem.Instance.AddReaction(increaseDamageGA);
    }
    // 检查是否拥有特定卡牌

}

