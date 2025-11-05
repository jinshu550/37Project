using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum HealthChangeType
{
    Damage,   // 伤害（扣血）
    Heal      // 治疗（加血）
}
public class ChangeHealthGA : GameAction
{
    // 目标对象（如敌人/玩家）
    public GameObject Target { get; private set; }
    // 变化值（正数=加血，负数=扣血）
    public int ChangeValue { get; private set; }
    // 变化类型（标记是伤害还是治疗）
    public HealthChangeType ChangeType { get; private set; }
    // 原始血量（用于后续计算和UI显示）
    public int OldHealth { get; private set; }
    // 变化后血量（用于后续计算和UI显示）
    public int NewHealth { get; private set; }

    /// <summary>
    /// 构造函数（创建血量变化动作）
    /// </summary>
    /// <param name="target">目标对象</param>
    /// <param name="changeValue">变化值（负数=扣血）</param>
    /// <param name="changeType">变化类型</param>
    public ChangeHealthGA(GameObject target, int changeValue, HealthChangeType changeType)
    {
        Target = target;
        ChangeValue = changeValue;
        ChangeType = changeType;
    }

    /// <summary>
    /// 内部更新血量数据（由BloodSystem调用，外部无需处理）
    /// </summary>
    public void UpdateHealthData(int oldHealth, int newHealth)
    {
        OldHealth = oldHealth;
        NewHealth = newHealth;
    }
}
/// <summary>
/// 死亡动作（当血量归零时触发）
/// </summary>
public class DeathGA : GameAction
{
    public GameObject Target { get; private set; }

    public DeathGA(GameObject target)
    {
        Target = target;
    }
}
