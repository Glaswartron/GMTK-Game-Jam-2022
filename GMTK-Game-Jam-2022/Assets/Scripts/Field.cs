using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum FieldType { Normal, Sand, Water, Ice}

public class Field : MonoBehaviour
{
    public FieldType fieldType;
    public int movementCost;
    protected bool highlighted;
    public List<Field> neighbours;

    private void Start()
    {
        neighbours = new List<Field>();
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
    }

    public void UnLight()
    {
        highlighted = false;
        //ToDo: Das Objekt Highlighten.
    }

    public virtual void OnSelected()
    {
        PODPlayerMovement.instance.SelectField(this);
        highlighted = false;
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
            for (int i = 0; i < neighbours.Count; i++)
            {
                // Wird eine Linie mit Dicke 2 zu diesem Nachbar gezeichnet
                Handles.DrawAAPolyLine(2, transform.position, neighbours[i].transform.position);
            }
        }
    }
}
