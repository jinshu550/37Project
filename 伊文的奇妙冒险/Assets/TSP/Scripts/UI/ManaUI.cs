using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManaUI : MonoBehaviour
{
    [SerializeField] private TMP_Text manaValue;
    public void UpdateCurrentMana(int currentMana) 
    {
        manaValue.text =Mathf.Max(currentMana,0).ToString();
    }
}
