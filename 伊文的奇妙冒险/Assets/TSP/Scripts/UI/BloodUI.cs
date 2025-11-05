using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BloodUI : MonoBehaviour
{
    [SerializeField] private Slider bloodProgress;
    [SerializeField] private TextMeshProUGUI bloodValues;
    public void UpdateBloodProgress(float currentBlood, float maxBlood)
    {
        if (bloodProgress == null)
        {
            Debug.LogError("BloodUI: 缺少血条Progress");
            return;
        }

        if (maxBlood <= 0)
        {
            bloodProgress.value = 0;
            return;
        }
        float healthPercentage = Mathf.Clamp01(currentBlood / maxBlood);
        bloodProgress.value = healthPercentage;
    }
    public void UpdateBloodValues(float currentBlood, float maxBlood)
    {
        if (bloodValues == null)
        {
            Debug.LogError("缺少血条Text");
        }
        if (maxBlood <= 0)
        {
            return;
        }
        bloodValues.text = $"{maxBlood}/{currentBlood}";
    }
}
