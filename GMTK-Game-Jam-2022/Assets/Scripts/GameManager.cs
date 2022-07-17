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
    public Sprite ExtraRollsSprite;
    public Sprite MapSprite;

    [Header("Main UI")]
    public GameObject Holder;
    public TextMeshProUGUI diceCount;
    public DiceList diceList;
    public TextMeshProUGUI rangeText;

    [Header("Dice Roll UI")]
    public GameObject diceRollPanel;
    public TextMeshProUGUI diceRollNumber;

    [Header("GameWon Stuff")]
    public GameObject gameWonScreen;

    private void Start()
    {
        instance = this;
        collectedDices = new List<Dice>();
        collectedDices.Add(defaultDice);

        AudioManager.instance.StartMusic();
    }

    private void Update()
    {
        GameManager.instance.SetRangeText(PlayerMovement.instance.GetCurrentRange());
    }

    public void IncreaseTraversedFields()
    {
        traversedFields++;
    }

    public void ToggleMovementCostVisuals()
    {
        AudioManager.instance.Play("Press");

        foreach (Transform child in transform)
        {
            Field childField = child.GetComponent<Field>();
            if (childField != null)
            {
                childField.ToggleMovementCostVisual();
            }
        }
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
            // TODO: Nach PlayerMovement am Ende vom Zug?
            GameOver();
        }
        diceCount.SetText(rolls.ToString()); //UI

        if(currentDice == null) //Sollte eigentlich NIE vorkommen
        {
            currentDice = defaultDice;
        }

        int res = currentDice.Roll();
        cummulatedEyes += res; //Für die Statistik

        StartCoroutine(DiceRollCo(res));

        Debug.Log("Würfelwurf: " + res, this);

        //PlayerMovement.instance.RollDice(res);

        return res;
    }

    private IEnumerator DiceRollCo(int res)
    {
        diceRollPanel.SetActive(true);

        float rollStartTimestamp = Time.time;
        while (Time.time - rollStartTimestamp < 2.5f)
        {
            diceRollNumber.SetText(currentDice.Roll().ToString());
            yield return new WaitForSeconds(0.15f);
        }

        diceRollNumber.SetText(res.ToString());

        yield return new WaitForSeconds(1f);

        PlayerMovement.instance.RollDice(res);

        rangeText.gameObject.SetActive(true);
        rangeText.SetText(res.ToString());
        diceRollPanel.SetActive(false);
    }

    public void SetRangeText(int value)
    {
        rangeText.SetText(value.ToString());
        if(value == 0)
        {
            rangeText.gameObject.SetActive(false);
        }
    }

    public void UnlockDice(Dice dice)
    {
        Debug.Log("UnlockDice wurde aufgerufen");
        if(collectedDices.Count < diceCapacity)
        {
            Debug.Log("Würfel sollte hinzugefügt werden");
            collectedDices.Add(dice);
            diceList.ActivateDice(collectedDices.Count - 1, dice.diceSprite, dice.diceHighlightedSprite, dice.dicePressedSprite);
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
                ItemUISetUp(ExtraRollsSprite, "You can roll the dice a few more times", item.value.ToString() + " extra rolls");
                break;
            case ItemKind.newDice:
                ItemUISetUp(item.dice.diceSprite, item.dice.description, item.dice.diceName);
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
                ItemUISetUp(MapSprite, "Neat, a piece of our world's map! With that you see more of our planet! Your sight will increase", "Map-Piece");
                break;
        }
    }

    public void ItemUISetUp(Sprite sprite, string text, string name)
    {
        itemImage.sprite = sprite;
        itemName.SetText("You obtained " + name);
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
        AudioManager.instance.StopMusic();
        gameOverScreen.SetActive(true);
    }

    public void Retry(TextMeshProUGUI t)
    {
        t.SetText("Created by Glaswartron and BelmontR / RBolton. Thanks for watching the Credits ^^");
        //ToDo: Resette alles
    }

    public void BackToMainMenu()
    {
        SceneFader.instance.SwitchToScene("StartScene", 3);
    }

    public void ShowScore(TextMeshProUGUI tmpro)
    {
        tmpro.text = "Accumulated Eye-Sum: " + cummulatedEyes.ToString() + "\n" + "Fields traversed: " + traversedFields.ToString();
    }
    #endregion

    public void WinGame()
    {
        AudioManager.instance.StopMusic();
        gameWonScreen.SetActive(true);
    }
}
