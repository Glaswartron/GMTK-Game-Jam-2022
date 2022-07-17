using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

public enum FieldType { Normal, Sand, Water, Ice}

[ExecuteInEditMode]
public class Field : MonoBehaviour
{
    public FieldType fieldType;
    public int movementCost;
    public Field[] neighbours;

    //Die beiden Sprites sind für alle Fields gleich
    public Sprite highlightedSprite;
    public Sprite highlightedWalkedOnSprite;

    //Die Sprite unterscheidet sich je nach FieldType
    public Sprite normalSprite;
    public Sprite walkedOnSprite;

    public Sprite hoverOverSprite;
    public Sprite hoverOverWalkedOnSprite;

    protected bool highlighted;
    protected bool walkedOn = false;

    public TextMeshProUGUI mcostVisual;

    private SpriteRenderer spriteRenderer;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void FieldAction(int playerDiceResult = 0)
    {
        //Muss von erbenden klassen implementiert werden, wenn man ein Sonderfeld bauen möchte, das besondere Sachen macht.
        //pdr = 0, weil nicht jedes Feld, das diese Methode implementiert, diesen Parameter auch benötigt, wenn doch, hier, habt ihr ihn.
    }

    public void Highlight()
    {
        highlighted = true;
        //ToDo: Das Objekt Highlighten.
        if (!walkedOn)
        {
            spriteRenderer.sprite = highlightedSprite;
        }
        else
        {
            spriteRenderer.sprite = highlightedWalkedOnSprite;
        }
    }

    public void Unhighlight()
    {
        highlighted = false;
        //ToDo: Das Objekt Unhighlighten.
        if (!walkedOn)
        {
            spriteRenderer.sprite = normalSprite;
        }
        else
        {
            spriteRenderer.sprite = walkedOnSprite;
        }
    }

    public void HoverLight()
    {
        //ToDo: Das Objekt Highlighten.
        if (!walkedOn)
        {
            spriteRenderer.sprite = hoverOverSprite;
        }
        else
        {
            spriteRenderer.sprite = hoverOverWalkedOnSprite;
        }
    }


    public virtual void OnSelected()
    {
        AudioManager.instance.Play("Press");

        PlayerMovement.instance.SelectField(this); // Unhighlighted die Felder
    }

    public void GetWalkedOn()   //Das muss im Player aufgerufen werden, wenn das Feld betreten wird
    {
        walkedOn = true;
    }

    public virtual void ToggleMovementCostVisual()
    {
        if(mcostVisual.gameObject.activeSelf)
        {
            mcostVisual.gameObject.SetActive(false);
        }
        else
        {
            mcostVisual.gameObject.SetActive(true);
            mcostVisual.SetText(movementCost.ToString());
        }
    }

    //Wichtig, damit das geht, brauchen die Fields Collider!!! Glaube ich
    private void OnMouseOver()
    {
        if(highlighted)
        {
            HoverLight();
            if (Input.GetMouseButtonDown(0))
            {
                OnSelected();
            }
        }
    }
    private void OnMouseExit()
    {
        if(highlighted)
        {
            Highlight();
        }
    }

    private void OnDrawGizmos()
    {
        if (neighbours != null)
        {
            // Für jeden Nachbar...
            for (int i = 0; i < neighbours.Length; i++)
            {
                // Wird eine Linie mit Dicke 2 zu diesem Nachbar gezeichnet
                Handles.DrawAAPolyLine(2, transform.position, neighbours[i].transform.position);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (Field neighbour in neighbours)
        {
            SerializedObject neighbourSO = new SerializedObject(neighbour);
            SerializedProperty neighbourNeighboursProp = neighbourSO.FindProperty("neighbours");
            neighbourNeighboursProp.DeleteArrayElementAtIndex(System.Array.IndexOf(neighbour.neighbours, this));
            neighbourSO.ApplyModifiedPropertiesWithoutUndo();
        }
    }

}
