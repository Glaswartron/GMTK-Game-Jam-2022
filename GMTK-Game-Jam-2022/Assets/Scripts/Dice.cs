using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dice : ScriptableObject
{
    public bool unlocked;
    public Sprite diceSprite;

    public abstract int Roll();

    public void Unlock()
    {
        unlocked = true;
        //ToDo: Vlt noch mehr
    }
}
