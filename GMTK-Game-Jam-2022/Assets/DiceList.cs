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

    public void ActivateDice(int index, Sprite sprite)
    {
        dices[index].SetActive(true);
        dices[index].GetComponent<Image>().sprite = sprite;
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
        if(!descriptionUI.activeSelf)
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
            GameManager.instance.SelectDice(i);
            //ToDo: Animationsfenster vom Würfelwurf und alles machen
            GameManager.instance.RollDice();
            descriptionUI.SetActive(false);
        }

    }

    private void ChangeDescription(int i)
    {
        name.SetText(GameManager.instance.collectedDices[i].name);
        descr.SetText(GameManager.instance.collectedDices[i].description);
    }
}
