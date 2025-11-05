using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsPlayGA : GameAction
{
    public GameAction Action { get; private set; }      //对应游戏动作（治疗 攻击 出牌）
    public string Name { get; private set; }            //对应音频名称
    public SoundsPlayGA(GameAction action, string name)
    {
        Action = action;
        Name = name;
    }

}
