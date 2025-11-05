using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializeReferenceEditor;

[CreateAssetMenu(menuName = "Data/Card")]
public class CardData : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public int Mana { get; private set; }
    [field: SerializeField] public Sprite Image { get; private set; }
    public FunctionCardType FunctionCardType;
    // [SerializeReference] private List<Effect> effects = new();
    // public List<Effect> Effects => effects;
    [field: SerializeReference, SR] public Effect ManualTargetEffect { get; private set; } = null;
    [field: SerializeField] public List<AutoTargetEffect> OtherEffects { get; private set; }
    [field: SerializeField] public CardType CardType { get; private set; }
}
public enum CardType
{
    Attack,
    Health,
    Defend
}
