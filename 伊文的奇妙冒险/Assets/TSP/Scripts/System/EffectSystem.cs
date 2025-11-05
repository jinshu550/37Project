using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<PerformEffectGA>(PerformEffectPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<PerformEffectGA>();
    }
    private IEnumerator PerformEffectPerformer(PerformEffectGA performEffectGA)
    {

        GameAction effectActtion = performEffectGA.Effect.GetGameAction(performEffectGA.Targets, HeroSystem.Instance.HeroView);
        ActionSystem.Instance.AddReaction(effectActtion);
        yield return null;
    }
}
