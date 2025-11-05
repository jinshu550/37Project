using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleRoundGameStateSystem : Singleton<SingleRoundGameStateSystem>
{
    public bool HasTakenDamage { get; private set; }
    public bool HasFullOfHealth { get; private set; }
    public bool HasHalfOfHealth { get; private set; }
    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<EnemyTurnGA>(ResetTurnState, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(ResetTurnState, ReactionTiming.POST);
    }
    private void ResetTurnState(EnemyTurnGA enemyTurnGA = null)
    {
        HasTakenDamage = false;
        HasFullOfHealth = false;
        HasHalfOfHealth = false;
    }
    public void MarkDamageTakenThisTurn()
    {
        HasTakenDamage = true;
    }
    public void MarkFullOfHealth()
    {
        HasFullOfHealth = true;
    }
    public void MarkHalfOfHealth()
    {
        HasHalfOfHealth = true;
    }
}
