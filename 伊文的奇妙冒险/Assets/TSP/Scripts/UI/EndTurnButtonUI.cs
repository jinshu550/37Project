using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButtonUI : MonoBehaviour
{

    private void Start()
    {
        ActionSystem.SubscribeReaction<EnemyTurnGA>(OnEnemyTurnGAPre, ReactionTiming.POST);
    }

    public void OnClick()
    {
        EnemyTurnGA enemyTurnGA = new();
        ActionSystem.Instance.Perform(enemyTurnGA);
    }

    public void OnEnemyTurnGAPre(GameAction gameAction)
    {

    }

    private void OnDestroy()
    {
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(OnEnemyTurnGAPre, ReactionTiming.POST);
    }
}
