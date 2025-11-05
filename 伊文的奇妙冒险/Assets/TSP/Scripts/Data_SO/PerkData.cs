using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializeReferenceEditor;
using System;
[CreateAssetMenu(menuName = "Data/PerkData")]
public class PerkData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeReference, SR] public PerkCondition PerkCondition { get; private set; }
    [field: SerializeReference, SR] public AutoTargetEffect AutoTargetEffect { get; private set; }
    [field: SerializeReference, SR] public AutoTargetEffect AdditionalEffect { get; private set; }
    [field: SerializeField] public bool UseAutoTarget { get; private set; } = true;
    [field: SerializeField] public bool UseActionCasterAsTarget { get; private set; } = false;
    [field: SerializeField] public List<String> TargetCardTitles { get; private set; } = new();
}

