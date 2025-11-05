using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddStatusEffectGA : GameAction
{
    public StatusEffectType StatusEffectType { get; private set; }
    public int StackCount { get; private set; }
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Target { get; private set; }
    public int Duration { get; private set; }
    public AddStatusEffectGA(StatusEffectType statusEffectType, int stackCount, List<CombatantView> targets, int duration = 1)
    {
        StatusEffectType = statusEffectType;
        StackCount = stackCount;
        Targets = targets;
        Duration = duration;
    }
    public AddStatusEffectGA(StatusEffectType statusEffectType, int stackCount, CombatantView target, int duration = 1)
    {
        StatusEffectType = statusEffectType;
        StackCount = stackCount;
        Target = target;
        Duration = duration;
    }
}
