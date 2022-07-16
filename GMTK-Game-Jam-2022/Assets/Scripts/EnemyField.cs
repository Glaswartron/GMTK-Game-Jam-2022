using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyField : Field
{
    [Header("Enemy Stuff")]
    public int HP;
    private Stats stats;
    //ToDo: Enemy Visual einbauen

    private void Start()
    {
        stats = new Stats(HP);  //Alles wird auf das Objekt ausgelagert, damit die Originalwerte nicht verändert werden müssen
    }

    public override void FieldAction(int playerDiceResult = 0)
    {

    }

    private bool Fight(int playerDiceResult)
    {
        //ToDo: Kampfanimation
        PlayerMovement.instance.SubtractRange(stats.GetHP());
        stats.ReduceHP(playerDiceResult);

        return stats.IsDead();
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

    //Eine innere Klasse, damit die originalen Werte nicht angefasst werden müssen. Vlt wichtig, wenn man später das Spiel resetten möchte
    private class Stats
    {
        private int HP;

        public int GetHP()
        {
            return HP;
        }

        public void ReduceHP(int dmg)
        {
            if(PlayerMovement.instance.sword)
            {
                dmg += PlayerMovement.instance.swordBonus;  //Schwert-Bonusschaden wird hier draufgerechnet
            }

            HP -= dmg;
            if(HP < 0) { HP = 0; }
        }

        public bool IsDead()
        {
            return HP == 0;
        }

        public Stats(int HP)
        {
            this.HP = HP;
        }
    }
}