using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy")]
public class EnemyData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int AttackPower { get; private set; }
    [field: SerializeField] public int DefendValue { get; private set; }
    [field: SerializeField] public int HealValue { get; private set; }
    [field: SerializeField] public List<CardData> Deck { get; private set; }
    [field: SerializeField] public string AnimatorPath { get; private set; }
    [field: SerializeField] public List<CardData> FunctionCards { get; private set; }
    private Animator animator;
}
