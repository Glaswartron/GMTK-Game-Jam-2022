using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyField : Field
{
    [Header("Enemy Stuff")]
    public int HP;
    public Stats stats;
    //ToDo: Enemy Visual einbauen

    private void Start()
    {
        stats = new Stats(HP);  // Alles wird auf das Objekt ausgelagert, damit die Originalwerte nicht ver�ndert werden m�ssen
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

    /// <summary>
    /// Eine innere Klasse, damit die originalen Werte nicht angefasst werden m�ssen.
    /// Vlt wichtig, wenn man sp�ter das Spiel resetten m�chte
    /// </summary>
    public class Stats
    {
        private int HP;

        public int GetHP()
        {
            return HP;
        }

        public void ReduceHP(int dmg)
        {
            if (PlayerMovement.instance.sword)
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