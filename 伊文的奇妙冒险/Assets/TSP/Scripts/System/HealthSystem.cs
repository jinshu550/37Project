using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthSystem : Singleton<HealthSystem>
{
    [SerializeField] private GameObject healthVFX;
    [SerializeField] private GameObject maxHealthVFX;
    public bool IsExtraHealth = false;
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<HealthGA>(HealthPerformer);
        ActionSystem.AttachPerformer<MaxHealthGA>(MaxHealthPerformer);
        ActionSystem.AttachPerformer<ExtraHealthGA>(ExtraHealthPerformer);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<HealthGA>();
        ActionSystem.DetachPerformer<MaxHealthGA>();
        ActionSystem.DetachPerformer<ExtraHealthGA>();
    }
    private IEnumerator HealthPerformer(HealthGA healthGA)
    {
        foreach (var target in healthGA.Targets)
        {
            if (IsExtraHealth)
            {
                target.Health(healthGA.Amount + 1);
            }
            else
            {
                target.Health(healthGA.Amount);
            }
            GameObject healPar = Instantiate(healthVFX, target.transform.position, Quaternion.identity);
            Destroy(healPar, 2f);
            yield return new WaitForSeconds(0.15f);

        }
    }
    private IEnumerator MaxHealthPerformer(MaxHealthGA maxHealthGA)
    {
        foreach (var target in maxHealthGA.Targets)
        {
            target.MaxHealth();
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator ExtraHealthPerformer(ExtraHealthGA extraHealthGA)
    {
        IsExtraHealth = true;
        yield return null;
    }

}
