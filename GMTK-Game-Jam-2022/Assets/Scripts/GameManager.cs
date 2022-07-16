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

    [Header("Dice Stuff")]
    [Tooltip("Die Anzahl an Würfelwürfen, die der Spieler hat")]
    public int rolls;
    //public Dice[] allDices;
    public List<Dice> collectedDices;
    public Dice defaultDice;
    private Dice currentDice;
    [Tooltip("Die Anzahl an Würfeln, die man tragen kann")]
    public int diceCapacity;

    [Header("Metrics")]
    private int cummulatedEyes;
    private int traversedFields;
    private int gold;

    [Header("Technical Stuff")]
    public Transform PlayerStartPosition;

    private void Start()
    {
        instance = this;
        collectedDices = new List<Dice>();
        collectedDices.Add(defaultDice);
    }

    public void IncreaseTraversedFields()
    {
        traversedFields++;
    }

    public void AddGold(int gold)
    {
        this.gold += gold;
    }

    #region Dice
    //Standardmäßig wird der Default Dice ausgewählt
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
        //diceCount.SetText(rolls.ToString()); //UI

        if(currentDice == null) //Sollte eigentlich NIE vorkommen
        {
            currentDice = defaultDice;
        }

        int res = currentDice.Roll();
        cummulatedEyes += res; //Für die Statistik

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
            Debug.Log("Du trägst schon " + diceCapacity + "Würfel mit dir");
            //ToDo: Eine Nope Animation spielen
        }
    }

    public bool DiceInventoryFull()
    {
        return collectedDices.Count == 4;
    }

    public void IncreaseRollsBy(int amount)
    {
        rolls += amount;
    }
    #endregion

    #region GameOver

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }

    public void Retry()
    {
        //ToDo: Resette alles
    }

    public void BackToMainMenu()
    {

    }

    public void ShowScore(TextMeshProUGUI tmpro)
    {
        tmpro.text = "Accumulated Eye-Sum: " + cummulatedEyes.ToString() + "\n" + "Fields traversed: " + traversedFields.ToString() + "\n" + "Gold:" + gold.ToString();
    }
    #endregion
}
