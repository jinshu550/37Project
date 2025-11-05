using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddStatusEffectEffect : Effect
{
    [SerializeField] private StatusEffectType statusEffectType;
    [SerializeField] private int stackCount;
    [SerializeField] private int duration;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        return new AddStatusEffectGA(statusEffectType, stackCount, targets, duration);
    }
}
