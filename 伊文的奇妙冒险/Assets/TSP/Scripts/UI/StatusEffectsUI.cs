using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
// 这里放需要使用UnityEditorInternal的代码
using UnityEditorInternal;
#endif
using UnityEngine;

public class StatusEffectsUI : MonoBehaviour
{
    [SerializeField] private StatusEffectUI statusEffectUIPrefab;
    [SerializeField]
    private Sprite armorSprite, maxHealthUpSprite, damage_CapSprite, armorPiercingSprite,
     reduceDefenseSprite, increasedDamageSprite, AngrySprite, FulfillSprite;
    [SerializeField] private List<StatusEffectType> hideStackCountForType = new();
    private Dictionary<StatusEffectType, StatusEffectUI> statusEffectUIs = new();
    public bool IsEnemy;

    // 新增：合并显示护甲和防御降低效果
    public void UpdateCombinedArmorUI(CombatantView combatant)
    {
        int armor = combatant.GetStatusEffectStacks(StatusEffectType.ARMOR);
        int reduceDefense = combatant.GetStatusEffectStacks(StatusEffectType.ReduceDefense);

        // 计算综合护甲值
        int effectiveArmor = armor - reduceDefense;

        // 移除单独的护甲和防御降低UI
        RemoveSpecificStatusEffectUI(StatusEffectType.ARMOR);
        RemoveSpecificStatusEffectUI(StatusEffectType.ReduceDefense);

        // 根据综合护甲值显示相应UI
        if (effectiveArmor != 0)
        {
            StatusEffectType displayType = effectiveArmor > 0 ? StatusEffectType.ARMOR : StatusEffectType.ReduceDefense;
            Sprite displaySprite = effectiveArmor > 0 ? armorSprite : reduceDefenseSprite;
            int displayStacks = Mathf.Abs(effectiveArmor);

            UpdateSpecificStatusEffectUI(displayType, displaySprite, displayStacks);
        }
    }

    public void UpdateStatusEffectUI(StatusEffectType statusEffectType, int stackCount)
    {
        if (stackCount == 0)
        {
            if (statusEffectUIs.ContainsKey(statusEffectType))
            {
                StatusEffectUI statusEffectUI = statusEffectUIs[statusEffectType];
                statusEffectUIs.Remove(statusEffectType);
                Destroy(statusEffectUI.gameObject);
            }
        }
        else
        {
            if (!statusEffectUIs.ContainsKey(statusEffectType))
            {
                StatusEffectUI statusEffectUI = Instantiate(statusEffectUIPrefab, transform, false);
                statusEffectUI.transform.SetParent(transform);
                statusEffectUI.transform.localScale = Vector3.one;
                if (IsEnemy)
                {
                    statusEffectUI.AdjustTextSize();
                }
                statusEffectUIs.Add(statusEffectType, statusEffectUI);
            }
            Sprite sprite = GetSpriteByType(statusEffectType);
            bool hideStack = hideStackCountForType.Contains(statusEffectType);
            statusEffectUIs[statusEffectType].Set(sprite, stackCount, hideStack);
        }
    }
    private Sprite GetSpriteByType(StatusEffectType statusEffectType)
    {
        return statusEffectType switch
        {
            StatusEffectType.ARMOR => armorSprite,
            StatusEffectType.MaxHealthUp => maxHealthUpSprite,
            StatusEffectType.Damage_CAP => damage_CapSprite,
            StatusEffectType.ReduceDefense => reduceDefenseSprite,
            StatusEffectType.IncreasedDamage => increasedDamageSprite,
            StatusEffectType.Angry => AngrySprite,
            StatusEffectType.Fulfill => FulfillSprite,
            _ => null,
        };
    }

    //AI写的 不必理会
    #region 
    // 移除指定类型的状态效果UI
    private void RemoveSpecificStatusEffectUI(StatusEffectType statusEffectType)
    {
        if (statusEffectUIs.ContainsKey(statusEffectType))
        {
            StatusEffectUI statusEffectUI = statusEffectUIs[statusEffectType];
            statusEffectUIs.Remove(statusEffectType);
            Destroy(statusEffectUI.gameObject);
        }
    }
    // 内部方法：更新指定类型的状态效果UI
    private void UpdateSpecificStatusEffectUI(StatusEffectType statusEffectType, Sprite sprite, int stackCount)
    {
        if (!statusEffectUIs.ContainsKey(statusEffectType))
        {
            StatusEffectUI statusEffectUI = Instantiate(statusEffectUIPrefab, transform, false);
            statusEffectUI.transform.SetParent(transform);
            statusEffectUI.transform.localScale = Vector3.one;
            if (IsEnemy)
            {
                statusEffectUI.AdjustTextSize();
            }
            statusEffectUIs.Add(statusEffectType, statusEffectUI);
        }
        bool hideStack = hideStackCountForType.Contains(statusEffectType);
        statusEffectUIs[statusEffectType].Set(sprite, stackCount, hideStack);
    }

    #endregion
}
