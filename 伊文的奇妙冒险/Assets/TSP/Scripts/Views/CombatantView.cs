using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;


public class CombatantView : MonoBehaviour
{
    //[SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] private BloodUI bloodProgress;
    [SerializeField] private StatusEffectsUI statusEffectsUI;
    [SerializeField] private TextMeshProUGUI takedamageText;
    [SerializeField] public RectTransform imageTransform;
    public int BaseMaxHealth { get; private set; }
    public int CurrentMaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    private Dictionary<StatusEffectType, int> statusEffects = new();
    Vector2 originalPos;
    protected void SetupBase(int health, Sprite image)
    {
        BaseMaxHealth = CurrentHealth = CurrentMaxHealth = health;
        //spriteRenderer.sprite = image;
        if (imageTransform != null)
            originalPos = imageTransform.anchoredPosition;
        bloodProgress.UpdateBloodProgress(CurrentHealth, CurrentMaxHealth);
        bloodProgress.UpdateBloodValues(CurrentHealth, CurrentMaxHealth);
    }
    public void Damage(int damageAmount)
    {
        int increasedDamage = GetStatusEffectStacks(StatusEffectType.IncreasedDamage);
        int remainingDamage = damageAmount + increasedDamage;
        int damageCapStacks = GetStatusEffectStacks(StatusEffectType.Damage_CAP);
        int reduceDefense = GetStatusEffectStacks(StatusEffectType.ReduceDefense);
        int initialDefense = GetStatusEffectStacks(StatusEffectType.ARMOR);
        if (damageCapStacks > 0)
        {
            remainingDamage = 1;
        }
        int currentArmor = initialDefense - reduceDefense;
        if (currentArmor > 0)
        {
            if (currentArmor >= damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, remainingDamage);
                remainingDamage = 0;
            }
            else if (currentArmor < damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, currentArmor);
                remainingDamage -= currentArmor;
            }
        }
        else if (currentArmor < 0)
        {
            remainingDamage += Mathf.Abs(currentArmor);
        }
        if (remainingDamage > 0)
        {
            CurrentHealth -= remainingDamage;
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }
            //实例化伤害UI
            TextMeshProUGUI text = Instantiate(takedamageText, transform.position, Quaternion.identity);
            text.text = $"-{remainingDamage}";
            text.gameObject.transform.SetParent(imageTransform, false);

            //标记扣血
            SingleRoundGameStateSystem.Instance.MarkDamageTakenThisTurn();
            if (CurrentHealth < CurrentMaxHealth / 2)
            {
                SingleRoundGameStateSystem.Instance.MarkHalfOfHealth();
            }

            StartCoroutine(DestroyTextAfterDelay(text.gameObject, 1f));
        }
        imageTransform.DOShakePosition(0.2f, 50f).OnComplete(() =>
    {
        // 动画结束后，强制把位置拉回初始值（彻底解决位移偏移）
        imageTransform.anchoredPosition = originalPos;
    });
        bloodProgress.UpdateBloodProgress(CurrentHealth, CurrentMaxHealth);
        bloodProgress.UpdateBloodValues(CurrentHealth, CurrentMaxHealth);
    }
    public void HalveArmor()
    {
        // 从状态效果获取当前护甲值
        int currentArmorFromStatus = GetStatusEffectStacks(StatusEffectType.ARMOR);
        int halvedArmor = currentArmorFromStatus / 2;

        // 更新状态效果中的护甲值
        RemoveStatusEffect(StatusEffectType.ARMOR, currentArmorFromStatus);
        if (halvedArmor >= 0)
        {
            AddStatusEffect(StatusEffectType.ARMOR, halvedArmor);
        }
    }

    //血量
    #region 
    public void Health(int healthAmount)
    {
        int health = CurrentHealth;
        if (CurrentHealth + healthAmount <= CurrentMaxHealth)
        {
            health = CurrentHealth + healthAmount;
        }
        else
        {
            AddStatusEffect(StatusEffectType.ARMOR, 1);
        }
        int maxHealth = GetStatusEffectStacks(StatusEffectType.MaxHealthUp);
        if (maxHealth > 0)
        {
            CurrentMaxHealth += maxHealth;
        }
        if (maxHealth == CurrentMaxHealth)
        {
            SingleRoundGameStateSystem.Instance.MarkFullOfHealth();
        }
        CurrentHealth = health;
        bloodProgress.UpdateBloodProgress(CurrentHealth, CurrentMaxHealth);
        bloodProgress.UpdateBloodValues(CurrentHealth, CurrentMaxHealth);
    }
    public void MaxHealth()
    {
        CurrentMaxHealth = BaseMaxHealth + GetStatusEffectStacks(StatusEffectType.MaxHealthUp);
        if (CurrentHealth <= 0)
        {
            RemoveStatusEffect(StatusEffectType.MaxHealthUp, GetStatusEffectStacks(StatusEffectType.MaxHealthUp));
        }
    }
    #endregion



    #region 
    public void AddStatusEffect(StatusEffectType type, int stackCount)
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] += stackCount;
        }
        else
        {
            statusEffects.Add(type, stackCount);
        }
        // 对于护甲和防御降低，更新合并显示
        if (type == StatusEffectType.ARMOR || type == StatusEffectType.ReduceDefense)
        {
            statusEffectsUI.UpdateCombinedArmorUI(this);
        }
        else
        {
            statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStacks(type));
        }
    }
    public void RemoveStatusEffect(StatusEffectType type, int stackCount)
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] -= stackCount;
            if (statusEffects[type] <= 0)
            {
                statusEffects.Remove(type);
            }
        }
        statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStacks(type));
    }
    public int GetStatusEffectStacks(StatusEffectType type)
    {
        if (statusEffects.ContainsKey(type)) return statusEffects[type];
        else return 0;
    }
    #endregion
    // 延迟销毁文本的协程
    private IEnumerator DestroyTextAfterDelay(GameObject textObject, float delay)
    {
        yield return new WaitForSeconds(delay); // 等待指定时间
        Destroy(textObject); // 销毁文本对象
    }
}
