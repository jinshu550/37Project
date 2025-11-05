using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpendManaGA : GameAction
{
    [SerializeField]public int Amount { get; set; }
    public SpendManaGA(int amount) 
    {
        Amount = amount;
    }
}
