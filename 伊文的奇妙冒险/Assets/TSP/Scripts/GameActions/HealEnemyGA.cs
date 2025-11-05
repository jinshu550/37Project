using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEnemyGA : GameAction, IHaveCaster
{
    public CombatantView Caster { get; private set; }

    public EnemyView Owner { get; private set; }
    public int Health { get; private set; }
    public HealEnemyGA(EnemyView owner, int health)
    {
        Caster = owner;
        Owner = owner;
        Health = health;
    }
}
