using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class EnemyView : CombatantView
{
    public string AnimatorPath { get; private set; }
    public Animator Animator { get; private set; }
    public List<CardData> Deck { get; private set; }
    public List<CardData> FunctionCards { get; private set; }
    public int AttackPower { get; set; }
    public int DefendValue { get; set; }
    public int HealValue { get; set; }
    private EnemyData EnemyData;
    public Sprite Image { get; private set; }
    public void SetUp(EnemyData enemyData)
    {
        EnemyData = enemyData;
        Image = enemyData.Image;
        AnimatorPath = enemyData.AnimatorPath;
        AttackPower = enemyData.AttackPower;
        DefendValue = enemyData.DefendValue;
        HealValue = enemyData.HealValue;
        Deck = enemyData.Deck;
        FunctionCards = enemyData.FunctionCards;
        SetupBase(enemyData.Health, enemyData.Image);
        UpdateImage();
        AdjustStatusUISize();
    }
    public EnemyData GetEnemyData()
    {
        return EnemyData;
    }
    private void UpdateImage()
    {
        GameObject go = transform.Find("CombatantViewBase/Canvas/Image").gameObject;
        Image replacedImage = go.GetComponent<Image>();
        if (Image != null)
            replacedImage.sprite = Image;
        if (AnimatorPath != null)
        {
            Animator currentAnimator = go.GetComponent<Animator>();
            RuntimeAnimatorController ctr = Resources.Load<RuntimeAnimatorController>(AnimatorPath);
            currentAnimator.runtimeAnimatorController = ctr;
        }
    }
    public void AdjustUILayerOrder()
    {
        Canvas uiCanvas = GetComponentInChildren<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        uiCanvas.worldCamera = Camera.main;
        uiCanvas.sortingLayerID = 2;
    }
    public void AdjustStatusUISize()
    {
        GameObject go = transform.Find("CombatantViewBase/Canvas/Image/StatusEffectsUI").gameObject;
        if (go != null)
        {
            go.GetComponent<StatusEffectsUI>().IsEnemy = true;
        }
        GridLayoutGroup glg = go.GetComponent<GridLayoutGroup>();
        glg.cellSize = glg.cellSize = new Vector2(150, 150);
        glg.spacing = new Vector2(60, 0);
    }
}
