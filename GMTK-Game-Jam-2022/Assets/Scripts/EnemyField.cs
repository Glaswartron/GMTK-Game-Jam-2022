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
        PlayerMovement.instance.SubtractRange(HP);
        if (HP < 0) { HP = 0; }

        return HP == 0;
    }

    public override void OnSelected()
    {
        if (Fight(PlayerMovement.instance.GetCurrentRange())) //Wird in der If-Clause ausgeführt
        {
            PlayerMovement.instance.SelectField(this);   //Spieler geht auf das Gegnerfeld rauf
        }
        else
        {
            PlayerMovement.instance.SelectField(PlayerMovement.instance.currentField);   //SPieler muss auf seinem aktuellem Feld bleiben
        }
        highlighted = false;
    }

}