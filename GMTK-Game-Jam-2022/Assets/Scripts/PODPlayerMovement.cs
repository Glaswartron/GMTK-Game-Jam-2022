using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState {Idle, Moving, WaitingForInput};

public class PODPlayerMovement : MonoBehaviour
{
    public static PODPlayerMovement instance;

    public float movementSpeed;
    public Field currentField;
    public PlayerState currentState;

    private int currentRange;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Moving:
                gameObject.transform.position = Vector3.MoveTowards(transform.position, currentField.transform.position, movementSpeed * Time.deltaTime);
                if(transform.position == currentField.transform.position)
                {
                    if (currentRange == 0)
                    {
                        currentState = PlayerState.Idle;
                    }
                    else
                    {
                        currentState = PlayerState.WaitingForInput;
                        HighlightFields();
                    }
                }
                break;
            case PlayerState.Idle:
                break;
            case PlayerState.WaitingForInput:
                break;
        }
    }

    public int RollDice(int min, int max)
    {
        currentRange = Random.Range(min, max);
        return currentRange;
    }

    public int GetCurrentRange()
    {
        return currentRange;
    }

    public void SubtractRange(int value)
    {
        currentRange -= value;
        if(currentRange < 0) { currentRange = 0; }
    }

    public void HighlightFields()
    {
        foreach (var field in currentField.neighbours)
        {
            if(field.movementCost <= currentRange)
            field.Highlight();
        }

        currentState = PlayerState.WaitingForInput;
    }

    //Muss vom Feld, das vom Spieler angeklickt wurde, aufgerufen werden.
    public void SelectField(Field selectedField)
    {
        foreach (var field in currentField.neighbours)
        {
            field.UnLight();
        }

        currentField = selectedField;
        currentRange -= selectedField.movementCost;
        currentState = PlayerState.Moving;
    }



}
