using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HealthEffect : Effect
{
    [SerializeField] private int healthAmount;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        HealthGA healthGA = new(healthAmount, targets, caster);
        return healthGA;
    }
}
