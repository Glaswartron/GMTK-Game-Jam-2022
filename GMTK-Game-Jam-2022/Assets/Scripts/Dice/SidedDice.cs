using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dice", menuName = "Dice/Sided Dice")]
public class SidedDice : Dice
{
    [Tooltip("Jeder Eintrag ist eine Würfelseite, die ")]
    public int[] Sides;

    public override int Roll()
    {
        return Sides[Random.Range(0, Sides.Length - 1)];
    }

}
