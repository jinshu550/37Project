using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_PiercingGA : GameAction
{
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Caster { get; private set; }
    public Armor_PiercingGA(List<CombatantView> targets, CombatantView caster)
    {
        Targets = targets;
        Caster = caster;
    }
}
