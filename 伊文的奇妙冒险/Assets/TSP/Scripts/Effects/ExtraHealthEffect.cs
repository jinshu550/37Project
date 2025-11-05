using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraHealthEffect : Effect
{
    [SerializeField] private int healthAmount;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        ExtraHealthGA extraHealthGA = new(healthAmount, targets, caster);
        return extraHealthGA;
    }
}
