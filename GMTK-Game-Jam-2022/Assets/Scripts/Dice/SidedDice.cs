using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidedDice : Dice
{
    [Tooltip("Jeder Eintrag ist eine Würfelseite, die ")]
    public int[] Sides;

    public override int Roll()
    {
        return Sides[Random.Range(0, Sides.Length - 1)];
    }

}
