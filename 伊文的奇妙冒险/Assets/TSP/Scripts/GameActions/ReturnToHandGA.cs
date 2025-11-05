using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToHandGA : GameAction
{
    public Card CardToHand { get; private set; }
    public ReturnToHandGA(Card card)
    {
        CardToHand = card;
    }
}
