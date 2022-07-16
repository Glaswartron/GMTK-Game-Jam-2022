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

    [Tooltip("Die Anzahl an W�rfelw�rfen, die der Spieler hat")]
    public int rolls;

    [Header("Dice Stuff")]
    //public Dice[] allDices;
    public List<Dice> collectedDices;
    public Dice defaultDice;
    private Dice currentDice;
    [Tooltip("Die Anzahl an W�rfeln, die man tragen kann")]
    public int diceCapacity;

    [Header("Metrics")]
    private int cummulatedEyes;
    private int traversedFields;

    private void Start()
    {
        instance = this;
        collectedDices = new List<Dice>();
        collectedDices[0] = defaultDice;
    }

    //Standardm��ig wird der Default Dice ausgew�hlt
    public void SelectDice(int i = 0)
    {
        currentDice = collectedDices[i];
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
        cummulatedEyes += res; //F�r die Statistik

        return res;
    }

    public void UnlockDice(Dice dice)
    {
        if(collectedDices.Count < diceCapacity)
        {
            collectedDices.Add(dice);
        }
        else
        {
            Debug.Log("Du tr�gst schon " + diceCapacity + "W�rfel mit dir");
            //ToDo: Eine Nope Animation spielen
        }
    }

    public void GameOver()
    {

    }
}
