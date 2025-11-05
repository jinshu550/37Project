using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MaxHandsEffect : Effect
{
    [SerializeField] private int extraHandCards;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        MaxHandCardsGA maxHandCardsGA = new(targets, caster, extraHandCards);
        return maxHandCardsGA;
    }
}
