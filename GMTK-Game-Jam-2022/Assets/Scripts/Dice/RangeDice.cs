using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dice", menuName = "Dice/Range Dice")]
public class RangeDice : Dice
{
    [Tooltip("Mindestzahl, die gewürfelt werden kann")]
    public int min;
    [Tooltip("Höchstzahl, die gewürfelt werden kann")]
    public int max;

    public override int Roll()
    {
        return Random.Range(min, max);
    }
}
