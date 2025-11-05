using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DrawCard : Effect
{
    public int drawAmount;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        DrawCardsGA drawCardsGA = new(drawAmount);
        return drawCardsGA;
    }
}
