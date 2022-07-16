using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemKind { Sword, Brett, SandBoots, lens, extraDice, Gold, CoinDice, OctoDice}

public class ItemField : Field
{
    [Header("Item-Stuff")]
    public ItemKind item;

    public override void FieldAction(int playerDiceResult = 0)
    {
        //Muss von erbenden klassen implementiert werden, wenn man ein Sonderfeld bauen m�chte, das besondere Sachen macht.
        //pdr = 0, weil nicht jedes Feld, das diese Methode implementiert, diesen Parameter auch ben�tigt, wenn doch, hier, habt ihr ihn.
    }

    public override void OnSelected()
    {
        PlayerMovement.instance.SelectField(this);
        highlighted = false;

        ApplyItem();
    }

    private void ApplyItem()
    {
        PlayerMovement player = PlayerMovement.instance;
        switch(item)    //Die ersten drei Cases beeinflussen das Movement vom Player direkt, die anderen nicht, die werden nict �ber den Player gehandelt
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
            case ItemKind.lens:
                break;
            case ItemKind.extraDice:
                break;
            case ItemKind.Gold:
                break;
            case ItemKind.CoinDice:
                break;
            case ItemKind.OctoDice:
                break;
        }
    }
}
