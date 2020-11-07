using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private string cardName;
    [SerializeField] private string suit;
    [SerializeField] private int value;
    [SerializeField] private bool drawn = false;
    public bool active = false;
    public static Card activeCard;

    public string getName()
    {
        return cardName;
    }

    public string getSuit()
    {
        return suit;
    }

    public int getValue()
    {
        return value;
    }

    public bool hasBeenDrawn()
    {
        return drawn;
    }

    public void setDrawn(bool val)
    {
        drawn = val;
    }

    public bool isActiveCard()
    {
        return active;
    }

    public void setActive()
    {
        active = true;
        Card.activeCard = this;
    }
}
