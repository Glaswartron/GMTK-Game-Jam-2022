using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemKind { Sword, Brett, SandBoots, lens, extraRoll, Gold, newDice} 

public class ItemField : Field
{
    [Header("Item-Stuff")]
    public ItemKind item;
    [Tooltip("Wenn man eine ItemKind ausgewählt hat, die einen Value hat, z.B. extraRoll oder Gold")]
    public int value;
    [Tooltip("Wird nur benutzt, wenn ItemKind = newDice")]
    public Dice dice;

    public GameObject ItemVisual;
    public bool collected = false;

    protected override void Start()
    {
        base.Start();

        ItemVisual.SetActive(true);
    }

    public override void FieldAction(int playerDiceResult = 0)
    {
        Debug.Log("FUUUUUUUUUUUUUUUUUUCK");
        if (!collected)
        {
            ApplyItem();
            PlayerMovement.instance.SetAnimationState("CollectItem");
            GameManager.instance.OpenItemUI(this);
        }
    }

    public override void OnSelected()
    {
        PlayerMovement.instance.SelectField(this);
        highlighted = false;

    }

    private void ApplyItem()
    {
        PlayerMovement player = PlayerMovement.instance;
        switch(item)    //Die ersten drei Cases beeinflussen das Movement vom Player direkt, die anderen nicht, die werden nict über den Player gehandelt
        {
            case ItemKind.Sword:
                if(player.GetSword())
                {
                    player.SetSword(true);
                }
                break;
            case ItemKind.Brett:
                if (player.GetBrett())
                {
                    player.SetBrett(true);
                }
                break;
            case ItemKind.SandBoots:
                if (player.GetBoots())
                {
                    player.SetSandBoots(true);
                }
                break;
            case ItemKind.newDice:
                if(!GameManager.instance.DiceInventoryFull())
                {
                    GameManager.instance.UnlockDice(dice);
                }
                break;
            case ItemKind.lens:
                break;
            case ItemKind.extraRoll:
                GameManager.instance.IncreaseRollsBy(value);
                break;
        }
        collected = true;
        ItemVisual.SetActive(false);
    }
}
