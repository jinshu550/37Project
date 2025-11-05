using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem : Singleton<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;
    private const int Base_Mana = 5;
    private int current_mana = Base_Mana;
    private int max_mana = Base_Mana;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<SpendManaGA>(SpendManaPerformer);
        ActionSystem.AttachPerformer<RefillManaGA>(RefillManaPerformer);
        ActionSystem.AttachPerformer<ExtraManaGA>(ExtraManaPerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<SpendManaGA>();
        ActionSystem.DetachPerformer<RefillManaGA>();
        ActionSystem.DetachPerformer<ExtraManaGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyTurnPostReaction, ReactionTiming.POST);
    }
    void Start()
    {
        manaUI.UpdateCurrentMana(current_mana);
    }
    public bool HasEnoughMana(int mana)
    {
        return current_mana >= mana && current_mana >= 0;
    }
    private IEnumerator SpendManaPerformer(SpendManaGA spendManaGA)
    {
        current_mana -= spendManaGA.Amount;
        manaUI.UpdateCurrentMana(current_mana);
        yield return null;
    }
    private IEnumerator RefillManaPerformer(RefillManaGA refillManaGA)
    {
        current_mana = max_mana;
        manaUI.UpdateCurrentMana(current_mana);
        yield return null;
    }
    private IEnumerator ExtraManaPerformer(ExtraManaGA extraManaGA)
    {
        max_mana = extraManaGA.Amount + Base_Mana;
        current_mana = max_mana;
        manaUI.UpdateCurrentMana(current_mana);
        yield return null;
    }
    private void EnemyTurnPostReaction(EnemyTurnGA enemyTurnGA)
    {
        RefillManaGA refillManaGA = new();
        ActionSystem.Instance.AddReaction(refillManaGA);
    }
}
