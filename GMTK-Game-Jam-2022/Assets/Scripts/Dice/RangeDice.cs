using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeDice : Dice
{
    [Tooltip("Mindestzahl, die gew�rfelt werden kann")]
    public int min;
    [Tooltip("H�chstzahl, die gew�rfelt werden kann")]
    public int max;

    public override int Roll()
    {
        return Random.Range(min, max);
    }
}
