using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInteraction : MonoBehaviour
{
    public GameObject statusEffectsUI;
    void Start()
    {
        // 初始时隐藏StatusEffectsUI
        if (statusEffectsUI != null)
        {
            statusEffectsUI.SetActive(false);
        }
    }

    // 当鼠标进入碰撞器时调用
    void OnMouseEnter()
    {
        if (statusEffectsUI != null)
        {
            statusEffectsUI.SetActive(true);
        }
    }

    // 当鼠标离开碰撞器时调用
    void OnMouseExit()
    {
        if (statusEffectsUI != null)
        {
            statusEffectsUI.SetActive(false);
        }
    }
}
