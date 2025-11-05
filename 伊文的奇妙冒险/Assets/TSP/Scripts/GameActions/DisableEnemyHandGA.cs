using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEnemyHandGA : GameAction
{
    public bool IsEnemyTurn { get; private set; } = true;
    public DisableEnemyHandGA() { }
}
