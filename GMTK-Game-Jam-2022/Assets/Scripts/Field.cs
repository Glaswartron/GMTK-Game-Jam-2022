using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum FieldType { Normal, Sand, Water, Ice}

[ExecuteInEditMode]
public class Field : MonoBehaviour
{
    public FieldType fieldType;
    public int movementCost;
    public Field[] neighbours;
    public GameObject highligther;

    //Die beiden Sprites sind für alle Fields gleich
    public Sprite highlightedSprite;
    public Sprite highlightedWalkedOnSprite;

    //Die Sprite unterscheidet sich je nach FieldType
    public Sprite normalSprite;
    public Sprite walkedOnSprite;

    protected bool highlighted;
    protected bool walkedOn = false;

    private SpriteRenderer spriteRenderer;

    private void Start()
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
        highligther.SetActive(true);
        //ToDo: Das Objekt Highlighten.
        if (walkedOn)
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
        highligther.SetActive(false);
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

    public virtual void OnSelected()
    {
        PlayerMovement.instance.SelectField(this); // Unhighlighted die Felder
    }

    public void GetWalkedOn()   //Das muss im Player aufgerufen werden, wenn das Feld betreten wird
    {
        walkedOn = true;
    }

    //Wichtig, damit das geht, brauchen die Fields Collider!!! Glaube ich
    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0) && highlighted)
        {
            OnSelected();
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
