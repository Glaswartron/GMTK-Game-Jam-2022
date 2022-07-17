using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyField : Field
{
    [Header("Enemy Stuff")]
    public int HP;
    public Stats stats;
    public GameObject enemyVisual;

    public TextMeshProUGUI HPVisual;
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
        HPVisual.SetText(stats.GetHP().ToString() + " HP");

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

    public override void ToggleMovementCostVisual()
    {
        if (mcostVisual.gameObject.activeSelf)
        {
            mcostVisual.gameObject.SetActive(false);
            HPVisual.gameObject.SetActive(false);
        }
        else
        {
            mcostVisual.gameObject.SetActive(true);
            mcostVisual.SetText(movementCost.ToString());

            HPVisual.gameObject.SetActive(true);
            HPVisual.SetText(stats.GetHP().ToString() + "HP");
        }
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