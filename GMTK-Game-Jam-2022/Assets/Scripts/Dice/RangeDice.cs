using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
