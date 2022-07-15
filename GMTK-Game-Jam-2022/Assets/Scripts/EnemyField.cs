using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyField : Field
{
    [Header("Enemy Stuff")]
    public int HP;
    //ToDo: Enemy Visual einbauen

    public override void FieldAction(int playerDiceResult = 0)
    {

    }

    private bool Fight(int playerDiceResult)
    {
        //ToDo: Kampfanimation
        HP -= playerDiceResult;
        PODPlayerMovement.instance.SubtractRange(HP);
        if (HP < 0) { HP = 0; }

        return HP == 0;
    }

    public override void OnSelected()
    {
        if (Fight(PODPlayerMovement.instance.GetCurrentRange())) //Wird in der If-Clause ausgeführt
        {
            PODPlayerMovement.instance.SelectField(this);   //Spieler geht auf das Gegnerfeld rauf
        }
        else
        {
            PODPlayerMovement.instance.SelectField(PODPlayerMovement.instance.currentField);   //SPieler muss auf seinem aktuellem Feld bleiben
        }
        highlighted = false;
    }

}