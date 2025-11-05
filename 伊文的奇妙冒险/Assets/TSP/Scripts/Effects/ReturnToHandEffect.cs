using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ReturnToHandEffect : Effect
{
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        if (Card.CurrentPlayingCard != null)
        {
            return new ReturnToHandGA(Card.CurrentPlayingCard);
        }
        return null;
    }
}
