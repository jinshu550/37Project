using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_PiercingEffect : Effect
{
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        Armor_PiercingGA armor_PiercingGA = new(targets, caster);
        return armor_PiercingGA;
    }
}
