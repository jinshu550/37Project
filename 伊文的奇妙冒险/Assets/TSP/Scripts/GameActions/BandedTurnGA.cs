using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandedTurnGA : GameAction
{
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Caster { get; private set; }
    public int Duration { get; private set; }
    public BandedTurnGA(List<CombatantView> targets, CombatantView caster, int duration)
    {
        Targets = targets;
        Caster = caster;
        Duration = duration;
    }
}

