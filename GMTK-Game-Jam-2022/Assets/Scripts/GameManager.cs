using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
        
    [Header("Gameobjects to manage")]
    public GameObject gameOverScreen;
    public GameObject playerUI;
    public TextMeshProUGUI diceCount;

    [Tooltip("Die Anzahl an Würfelwürfen, die der Spieler hat")]
    public int rolls;

    [Header("Dice Stuff")]
    public Dice[] dices;
    private Dice currentDice;

    [Header("Metrics")]
    private int cummulatedEyes;
    private int traversedFields;

    private void Start()
    {
        instance = this;
    }

    //Standardmäßig wird der Default Dice ausgewählt
    public void SelectDice(int i = 0)
    {
        currentDice = dices[i];
    }

    public int RollDice()
    {
        rolls--;
        if(rolls == 0)
        {
            GameOver();
        }
        diceCount.SetText(rolls.ToString()); //UI

        int res = currentDice.Roll();
        cummulatedEyes += res; //Für die Statistik

        return res;
    }

    public void GameOver()
    {

    }
}
