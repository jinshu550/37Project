using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseDamageGA : GameAction
{
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Caster { get; private set; }
    public int Amount { get; private set; }
    public IncreaseDamageGA(List<CombatantView> targets, int amount)
    {
        Targets = targets;
        Amount = amount;
    }
}
