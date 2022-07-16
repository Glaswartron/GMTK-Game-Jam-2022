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

    public bool sword = false;
    public bool brett = false;
    public bool sandboots = false;

    public int sandBootsBonus = 1;
    public int brettBonus = 1;
    public int swordBonus = 1;

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
                    Debug.Log("Würfelzahl: " + currentRange, this);

                    bool canMove = HighlightFields();
                    if (canMove)
                        currentState = PlayerState.WaitingForInput;
                    else // Wenn der Spieler sich mit seiner Range nicht mehr bewegen kann
                    {
                        // TODO: Dem Spieler anzeigen, dass er verkackt hat
                        Debug.Log("Verkackt! Neu würfeln!", this);

                        currentRange = 0;
                        currentState = PlayerState.Idle;
                    }
                }

#if UNITY_EDITOR
                for (int num = 1; num <= 9; num++) {
                    if (Input.GetKeyDown((KeyCode)(48 + num))) {
                        currentRange = num; // !
                        Debug.Log("Würfelzahl: " + currentRange, this);

                        bool canMove = HighlightFields();
                        if (canMove)
                            currentState = PlayerState.WaitingForInput;
                        else // Wenn der Spieler sich mit seiner Range nicht mehr bewegen kann
                        {
                            // TODO: Dem Spieler anzeigen, dass er verkackt hat
                            Debug.Log("Verkackt! Neu würfeln!", this);

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
                        Debug.Log("Jetzt muss neu gewürfelt werden!", this);
                        currentState = PlayerState.Idle;
                    }
                    else
                    {
                        Debug.Log("Verbleibende Züge: " + currentRange, this);

                        bool canMove = HighlightFields();
                        if (canMove)
                            currentState = PlayerState.WaitingForInput;
                        else // Wenn der Spieler sich mit seiner Range nicht mehr bewegen kann
                        {
                            // TODO: Dem Spieler anzeigen, dass er verkackt hat
                            Debug.Log("Verkackt! Neu würfeln!", this);

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
    /// Generiert einen zufälligen int aus [minInclusive..maxInclusive]
    /// </summary>
    public int RollDice(int min, int max)
    {
        //WICHTIG: Die SelectDice() Methode muss vor dieser Methode aufgerufen werden! Über die UI oder so wahrscheinlich einfach
        currentRange = GameManager.instance.RollDice();
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

    public bool CheckDistance(Field field)
    {
        switch(field.fieldType)
        {
            case FieldType.Sand:
                if(sandboots)
                {
                    return field.movementCost - sandBootsBonus <= currentRange;
                }
                break;
            case FieldType.Water:
                if(brett)
                {
                    return field.movementCost - brettBonus <= currentRange;
                }
                break;
        }
        return field.movementCost <= currentRange;
    }

    /// <summary>
    /// Markiert alle benachbarten Felder, auf die der Spieler sich
    /// basierend auf <see cref="currentRange"/> bewegen kann
    /// </summary>
    /// <returns>Ob überhaupt ein Feld gehighlighted wurde. Das heißt, 
    /// ob der Spieler sich bewegen kann</returns>
    public bool HighlightFields()
    {
        bool somethingHighlighted = false;
        foreach (var field in currentField.neighbours)
        {
            if (CheckDistance(field))
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
        currentRange -= CalculateMovementCost(selectedField);

        GameManager.instance.IncreaseTraversedFields();

        MoveTo(currentField.transform.position);
    }

    private int CalculateMovementCost(Field field)
    {
        switch (field.fieldType)
        {
            case FieldType.Sand:
                if (sandboots)
                {
                    return field.movementCost - sandBootsBonus;
                }
                break;
            case FieldType.Water:
                if (brett)
                {
                    return field.movementCost - brettBonus;
                }
                break;
        }
        return field.movementCost;
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
