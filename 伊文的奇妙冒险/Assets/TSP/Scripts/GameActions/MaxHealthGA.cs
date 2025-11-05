using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxHealthGA : GameAction
{
    public int Amount { get; private set; }
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Caster { get; private set; }
    public MaxHealthGA(List<CombatantView> targets, CombatantView caster)
    {
        Targets = targets;
        Caster = caster;
    }
}
