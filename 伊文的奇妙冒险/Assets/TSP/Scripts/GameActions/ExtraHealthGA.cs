using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraHealthGA : GameAction
{
    public int Amount { get; private set; }
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Caster { get; private set; }
    public ExtraHealthGA(int amount, List<CombatantView> targets, CombatantView caster)
    {
        Amount = amount;
        Targets = new(targets);
        Caster = caster;
    }

}
