using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DefenseUI : MonoBehaviour
{
    [SerializeField] private Slider defenseProgress;

    public void UpdateDefenseProgress(float maxDefense, float currentDefense)
    {
        // 防止除零错误和空引用
        if (defenseProgress == null)
        {
            Debug.LogError("BloodUI: bloodProgress未赋值！");
            return;
        }

        if (maxDefense <= 0)
        {
            Debug.LogWarning("最大血量不能为0或负数");
            defenseProgress.value = 0;
            return;
        }

        float defensePercentage = Mathf.Clamp01(currentDefense / maxDefense); // 确保在0-1范围内
        defenseProgress.value = defensePercentage;
    }
}
