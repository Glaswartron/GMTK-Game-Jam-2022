using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
        
    [Header("Gameobjects to manage")]
    public GameObject gameOverScreen;
    public GameObject playerUI;

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

    [Header("Technical Stuff")]
    public Transform PlayerStartPosition;

    [Header("Item UI")]
    public GameObject itemUI;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Image itemImage;
    public Sprite BootsSprite;
    public Sprite SwordSprite;
    public Sprite BrettSprite;

    [Header("Main UI")]
    public GameObject Holder;
    public TextMeshProUGUI diceCount;
    public DiceList diceList;

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

        Debug.Log("Würfelwurf: " + res, this);

        PlayerMovement.instance.RollDice(res);

        return res;
    }

    public void UnlockDice(Dice dice)
    {
        if(collectedDices.Count < diceCapacity)
        {
            collectedDices.Add(dice);
            diceList.ActivateDice(collectedDices.Count - 1, dice.diceSprite);
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

    #region Item UI
    public void OpenItemUI(ItemField item)
    {
        itemUI.SetActive(true);
        switch (item.item)
        {
            case ItemKind.extraRoll:
                ItemUISetUp(null, "You can roll the dice a few more times", item.value.ToString() + " extra rolls");
                break;
            case ItemKind.newDice:
                ItemUISetUp(item.dice.diceSprite, item.dice.description, item.dice.name);
                break;
            case ItemKind.Brett:
                ItemUISetUp(BrettSprite, "Neat, a piece of wood! With that you can ride waves! Your water-traversal costs will decrease by "+ PlayerMovement.instance.brettBonus.ToString()+".", "Wooden Board");
                break;
            case ItemKind.Sword:
                ItemUISetUp(SwordSprite, "Neat, a better sword! With that you can fight enemies better! The damage you deal is increased by " + PlayerMovement.instance.swordBonus.ToString() + ".", "Hero's Sword");
                break;
            case ItemKind.SandBoots:
                ItemUISetUp(BootsSprite, "Neat, better boots! With them you can glide over sand! Your sand-traversal costs will decrease by " + PlayerMovement.instance.sandBootsBonus.ToString() + ".", "Desert Boots");
                break;
            case ItemKind.lens:
                ItemUISetUp(null, "Neat, a piece of our world's map! With that you see more of our planet! Your sight will", "Map-Piece");
                break;
        }
    }

    public void ItemUISetUp(Sprite sprite, string text, string name)
    {
        itemImage.sprite = sprite;
        itemName.SetText(name);
        itemDescription.SetText(text);
    }

    public void ItemUIButtonEvent()
    {
        itemUI.SetActive(false);
        PlayerMovement.instance.HoldUpItem(false);
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
        tmpro.text = "Accumulated Eye-Sum: " + cummulatedEyes.ToString() + "\n" + "Fields traversed: " + traversedFields.ToString();
    }
    #endregion
}
