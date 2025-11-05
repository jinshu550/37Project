using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasTakenDamageCondition : PerkCondition
{
    public override bool SubConditionIsMet(GameAction gameAction)
    {
        return SingleRoundGameStateSystem.Instance.HasTakenDamage;
    }

    public override void SubscribeCondition(Action<GameAction> reaction)
    {
        ActionSystem.SubscribeReaction<PlayCardGA>(reaction, ReactionTiming.PRE);
    }

    public override void UnsubscribeCondition(Action<GameAction> reaction)
    {
        ActionSystem.SubscribeReaction<PlayCardGA>(reaction, ReactionTiming.PRE);
    }


}
