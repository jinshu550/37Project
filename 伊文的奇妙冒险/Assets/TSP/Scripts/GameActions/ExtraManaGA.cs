using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraManaGA : GameAction
{
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Caster { get; private set; }
    public int Amount { get; private set; }
    public ExtraManaGA(List<CombatantView> targets, CombatantView caster, int amount)
    {
        Targets = targets;
        Caster = caster;
        Amount = amount;
    }
}
