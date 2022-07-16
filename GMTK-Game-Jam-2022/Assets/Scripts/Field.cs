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

    protected bool highlighted;

    private void Start()
    {

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
    }

    public void Unhighlight()
    {
        highlighted = false;
        highligther.SetActive(false);
        //ToDo: Das Objekt Unhighlighten.
    }

    public virtual void OnSelected()
    {
        PlayerMovement.instance.SelectField(this); // Unhighlighted die Felder
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
