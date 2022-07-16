using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PlayerState {Idle, Moving, WaitingForInput, WaitingForField};

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public float movementSpeed;
    public Field currentField;
    public PlayerState currentState;

    private Vector2 target;

    private bool sword = false;
    private bool brett = false;
    private bool sandboots = false;

    private int currentRange;

    private void Start()
    {
        instance = this;

        // Position vom Spieler in die Mitte vom ersten Feld setzen
        transform.position = currentField.transform.position;

        currentState = PlayerState.Idle;
    }

    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Moving:
                break;
            case PlayerState.Idle:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentRange = RollDice(1, 6); // !
                    Debug.Log("W�rfelzahl: " + currentRange, this);

                    bool canMove = HighlightFields();
                    if (canMove)
                        currentState = PlayerState.WaitingForInput;
                    else // Wenn der Spieler sich mit seiner Range nicht mehr bewegen kann
                    {
                        // TODO: Dem Spieler anzeigen, dass er verkackt hat
                        Debug.Log("Verkackt! Neu w�rfeln!", this);

                        currentRange = 0;
                        currentState = PlayerState.Idle;
                    }
                }

#if UNITY_EDITOR
                for (int num = 1; num <= 9; num++) {
                    if (Input.GetKeyDown((KeyCode)(48 + num))) {
                        currentRange = num; // !
                        Debug.Log("W�rfelzahl: " + currentRange, this);

                        bool canMove = HighlightFields();
                        if (canMove)
                            currentState = PlayerState.WaitingForInput;
                        else // Wenn der Spieler sich mit seiner Range nicht mehr bewegen kann
                        {
                            // TODO: Dem Spieler anzeigen, dass er verkackt hat
                            Debug.Log("Verkackt! Neu w�rfeln!", this);

                            currentRange = 0;
                            currentState = PlayerState.Idle;
                        }
                        break;
                    }
                }
#endif
                break;
            case PlayerState.WaitingForInput:
                break;
            case PlayerState.WaitingForField:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case PlayerState.Moving:
                gameObject.transform.position 
                    = Vector2.MoveTowards(transform.position, target, movementSpeed * Time.fixedDeltaTime);
                
                if ((Vector2)transform.position == target)
                {
                    if (currentRange == 0)
                    {
                        Debug.Log("Jetzt muss neu gew�rfelt werden!", this);
                        currentState = PlayerState.Idle;
                    }
                    else
                    {
                        Debug.Log("Verbleibende Z�ge: " + currentRange, this);

                        bool canMove = HighlightFields();
                        if (canMove)
                            currentState = PlayerState.WaitingForInput;
                        else // Wenn der Spieler sich mit seiner Range nicht mehr bewegen kann
                        {
                            // TODO: Dem Spieler anzeigen, dass er verkackt hat
                            Debug.Log("Verkackt! Neu w�rfeln!", this);

                            currentRange = 0;
                            currentState = PlayerState.Idle;
                        }

                    }
                }
                break;
        }
    }

    public void MoveTo(Vector2 target)
    {
        this.target = target;
        currentState = PlayerState.Moving;
    }

    /// <summary>
    /// Generiert einen zuf�lligen int aus [minInclusive..maxInclusive]
    /// </summary>
    public int RollDice(int min, int max)
    {
        currentRange = Random.Range(min, max+1);
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

    /// <summary>
    /// Markiert alle benachbarten Felder, auf die der Spieler sich
    /// basierend auf <see cref="currentRange"/> bewegen kann
    /// </summary>
    /// <returns>Ob �berhaupt ein Feld gehighlighted wurde. Das hei�t, 
    /// ob der Spieler sich bewegen kann</returns>
    public bool HighlightFields()
    {
        bool somethingHighlighted = false;
        foreach (var field in currentField.neighbours)
        {
            if (field.movementCost <= currentRange)
            {
                field.Highlight();
                somethingHighlighted = true;
            }
        }

        return somethingHighlighted;
    }

    /// <summary>
    /// Muss vom Feld, das vom Spieler angeklickt wurde, aufgerufen werden.
    /// </summary>
    public void SelectField(Field selectedField)
    {
        if (currentState != PlayerState.WaitingForInput)
            return;

        foreach (var field in currentField.neighbours)
        {
            field.Unhighlight();
        }

        currentField = selectedField;
        currentRange -= selectedField.movementCost;

        MoveTo(currentField.transform.position);
    }

    #region Getter und Setter
    public bool GetSword()
    {
        return sword;
    }

    public bool GetBrett()
    {
        return brett;
    }

    public bool GetBoots()
    {
        return sandboots;
    }

    public void SetSword(bool state)
    {
        sword = state;
    }

    public void SetBrett(bool state)
    {
        brett = state;
    }

    public void SetSandBoots(bool state)
    {
        sandboots = state;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Vector2 pos = (Vector2)transform.position + new Vector2(0.5f, 0.5f);
        Handles.Label(pos, new GUIContent($"State: {currentState.ToString()}\nRange: {currentRange}"));
    }
}
