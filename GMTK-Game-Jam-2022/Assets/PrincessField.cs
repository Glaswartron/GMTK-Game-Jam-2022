using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessField : Field
{

    public virtual void FieldAction(int playerDiceResult = 0)
    {
        GameManager.instance.WinGame();
    }
}
