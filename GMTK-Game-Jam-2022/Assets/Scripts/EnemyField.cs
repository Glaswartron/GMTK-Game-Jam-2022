using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyField : Field
{
    [Header("Enemy Stuff")]
    public int HP;
    public Stats stats;
    public GameObject enemyVisual;
    //ToDo: Enemy Visual einbauen

    private Animator animator;

    protected override void Start()
    {
        base.Start();

        stats = new Stats(HP);  // Alles wird auf das Objekt ausgelagert, damit die Originalwerte nicht verändert werden müssen

        animator = GetComponent<Animator>();
    }

    public override void FieldAction(int playerDiceResult = 0)
    {

    }

    public void TakeHit()
    {
        int hitAnimIdx = Random.Range(0, 2);
        animator.Play("EnemyHit"+hitAnimIdx, 0);

        stats.ReduceHP(1);

        if (stats.IsDead())
        {
            StartCoroutine(DisableVisualAfterDelay(0.8f));
        }
    }

    private IEnumerator DisableVisualAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        enemyVisual.SetActive(false);
    }

    /*private bool Fight(int playerDiceResult)
    {
        //ToDo: Kampfanimation
        PlayerMovement.instance.SubtractRange(stats.GetHP());
        stats.ReduceHP(playerDiceResult);

        return stats.IsDead();
    }*/

    /// <summary>
    /// Eine innere Klasse, damit die originalen Werte nicht angefasst werden müssen.
    /// Vlt wichtig, wenn man später das Spiel resetten möchte
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