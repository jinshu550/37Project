using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameAction
{
    public List<GameAction> PreReactions { get; private set; } = new();     //动作前发生
    public List<GameAction> PerformReactions { get; private set; } = new(); //动作中发生
    public List<GameAction> PostReactions { get; private set; } = new();    //动作后发生
}
