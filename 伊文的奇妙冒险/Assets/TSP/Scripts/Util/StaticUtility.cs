using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class StaticUtility
{
    public static List<EnemyData> CurrentEnemyData;  //当前战斗场景的敌人数据

    public static CardData CurrentEnemyBuff;//怪物功能牌
    
    //public static List<string> BattleCards = new();//出战功能
    public static void ClearPartData()
    {
        CurrentEnemyData = null;
        CurrentEnemyBuff = null;
    }
}
