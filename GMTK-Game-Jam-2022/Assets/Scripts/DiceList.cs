using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceList : MonoBehaviour
{
    public GameObject[] dices;

    public GameObject descriptionUI;
    public TextMeshProUGUI name;
    public TextMeshProUGUI descr;
    private int currentDice;

    private int state = 0;

    private void Start()
    {
        foreach(var d in dices)
        {
            d.SetActive(false);
        }
        dices[0].SetActive(true);
    }

    public void ActivateDice(int index, Sprite sprite, Sprite highlightedSprite, Sprite pressedSprite)
    {
        dices[index].SetActive(true);
        Button diceButton = dices[index].GetComponent<Button>();
        diceButton.targetGraphic.GetComponent<Image>().sprite = sprite;

        if (highlightedSprite != null && pressedSprite != null)
        {
            SpriteState spriteState = new SpriteState();
            if (highlightedSprite != null)
                spriteState.highlightedSprite = highlightedSprite;
            if (pressedSprite != null)
                spriteState.pressedSprite = pressedSprite;

            diceButton.spriteState = spriteState;
        }
        
    }

    public void ToggleAnimationState()
    {
        if (state == 0)
        {
            GetComponent<Animator>().Play("Away");
            state = 1;
        }
        else
        {
            GetComponent<Animator>().Play("Out");
            state = 0;
        }
    }

    public void ButtonEvent(int i)
    {
        if (PlayerMovement.instance.currentState != PlayerState.Idle)
            return;

        if (!descriptionUI.activeSelf)
        {
            descriptionUI.SetActive(true);
            ChangeDescription(i);
            currentDice = i;
        }
        else if(descriptionUI.activeSelf && currentDice != i)
        {
            ChangeDescription(i);
        }
        else if(descriptionUI.activeSelf && currentDice == i)
        {
            if (PlayerMovement.instance.currentState != PlayerState.Idle)
                return;

            PlayerMovement.instance.currentState = PlayerState.WaitingForDice;

            GameManager.instance.SelectDice(i);
            GameManager.instance.RollDice();
            descriptionUI.SetActive(false);
        }

    }

    private void ChangeDescription(int i)
    {
        name.SetText(GameManager.instance.collectedDices[i].diceName);
        descr.SetText(GameManager.instance.collectedDices[i].description);
    }
}
