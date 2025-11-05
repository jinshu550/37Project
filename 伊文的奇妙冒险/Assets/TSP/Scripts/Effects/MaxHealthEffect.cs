using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MaxHealthEffect : Effect
{
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        MaxHealthGA maxHealthEffect = new(targets, caster);
        return maxHealthEffect;
    }
}
