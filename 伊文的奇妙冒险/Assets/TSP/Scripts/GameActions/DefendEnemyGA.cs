using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendEnemyGA : GameAction, IHaveCaster
{
    public CombatantView Caster { get; private set; }

    public EnemyView Owner { get; private set; }
    public int Armor { get; private set; }
    public DefendEnemyGA(EnemyView owner, int armor)
    {
        Caster = owner;
        Owner = owner;
        Armor = armor;
    }
}
