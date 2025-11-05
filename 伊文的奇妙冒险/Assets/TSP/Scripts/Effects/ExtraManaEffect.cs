using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraManaEffect : Effect
{
    [SerializeField] private int amount;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        ExtraManaGA extraManaGA = new(targets, caster, amount);
        return extraManaGA;
    }
}
