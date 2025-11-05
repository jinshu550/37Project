using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlannedCard
{   //要行动的敌人
    public EnemyView Enemy { get; private set; }
    //计划使用的卡牌列表
    public List<CardData> PlannedCards { get; private set; }
    //构造函数：
    public EnemyPlannedCard(EnemyView enemy, List<CardData> plannedCards)
    {
        Enemy = enemy;
        PlannedCards = plannedCards;
    }
}

