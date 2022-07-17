using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Dice", menuName = "Dice")]
public abstract class Dice : ScriptableObject
{
    public bool unlocked;

    public Sprite diceSprite;
    public Sprite diceHighlightedSprite;
    public Sprite dicePressedSprite;

    public string description;
    public string diceName;

    public abstract int Roll();

    public void Unlock()
    {
        unlocked = true;
        //ToDo: Vlt noch mehr
    }
}
