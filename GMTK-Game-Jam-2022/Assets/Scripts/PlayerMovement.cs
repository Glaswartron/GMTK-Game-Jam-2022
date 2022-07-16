using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PlayerState {Idle, Moving, WaitingForInput, WaitingForField, Attacking};

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public float movementSpeed;
    public Field currentField;
    private Field targetField;
    public PlayerState currentState;

    private Vector2 target;

    public bool sword = false;
    public bool brett = false;
    public bool sandboots = false;

    public int sandBootsBonus = 1;
    public int brettBonus = 1;
    public int swordBonus = 1;

    private int currentRange;

    private Action<Field> moveToCallback;

    private Animator animator;

    private void Start()
    {
        instance = this;

        animator = GetComponent<Animator>();

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
                    currentRange = RollDice(currentRange); // ! Muss rausgenommen werden
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
                    moveToCallback.Invoke(targetField);

                break;
        }
    }

    /// <summary>
    /// <paramref name="target"/> kann alles sein
    /// </summary>
    public void MoveTo(Vector2 target, Action<Field> callback = null)
    {
        this.target = target;
        currentState = PlayerState.Moving;

        moveToCallback = callback;
    }

    /// <summary>
    /// Generiert einen zuf�lligen int aus [minInclusive..maxInclusive]
    /// </summary>
    public int RollDice(int diceResultFromGM)
    {
        //WICHTIG: Die SelectDice() Methode muss vor dieser Methode aufgerufen werden! �ber die UI oder so wahrscheinlich einfach
        //ToDo: Das ganze hier vlt nochmal anders machen. Den Spieler nicht direkt w�rfeln lassen und sowas
        currentRange = diceResultFromGM;
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
    /// <returns>Ob �berhaupt ein Feld gehighlighted wurde. Das hei�t, 
    /// ob der Spieler sich bewegen kann</returns>
    public bool HighlightFields()
    {
        bool somethingHighlighted = false;
        Debug.Log(currentField);
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

        targetField = selectedField;

        foreach (var field in currentField.neighbours)
        {
            field.Unhighlight();
        }

        if (!(selectedField is EnemyField) || ((EnemyField)selectedField).stats.GetHP() <= 0)
        {
            //currentField = selectedField;
            currentRange -= CalculateMovementCost(selectedField);

            GameManager.instance.IncreaseTraversedFields();

            MoveTo(selectedField.transform.position, OnFieldReached);
        } 
        else
        {
            Vector2 hereToThereDir = (selectedField.transform.position - currentField.transform.position).normalized;


            MoveTo((Vector2)selectedField.transform.position - hereToThereDir, OnEnemyFieldReached);
        }
    }

    private void OnFieldReached(Field field)
    {
        field.GetWalkedOn();

        if (field != currentField)
        {
            field.FieldAction();
            Debug.Log("Hi");
        }

        currentField = field;

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

    private void OnEnemyFieldReached(Field field)
    {
        EnemyField enemyField = field as EnemyField;

        StartCoroutine(AttackCo(enemyField));
    }

    private IEnumerator AttackCo(EnemyField enemyField)
    {
        currentState = PlayerState.Attacking;

        // Die Zahl an Schl�gen bis die Moves verbaucht sind oder der Gegner besiegt ist
        int hits = Mathf.Min(currentRange, enemyField.stats.GetHP()); 

        // F�r jeden Hit einmal die Animation abspielen und f�r die L�nge der Animation warten
        for (int i = 0; i < hits; i++)
        {
            SetAnimationState("PlayerAttackDown");
            float animDuration = animator.GetCurrentAnimatorStateInfo(0).length;

            currentRange--;

            yield return new WaitForSeconds(animDuration);
        }

        if (enemyField.stats.IsDead())
        {
            targetField = enemyField;
            MoveTo(enemyField.transform.position, OnFieldReached);
        }
        else
        {
            targetField = currentField;
            MoveTo(currentField.transform.position, OnFieldReached);
        }
    }

    /// <summary>
    /// Wird aus dem Animator aufgerufen
    /// </summary>
    public void HitEnemy()
    {
        ((EnemyField)targetField).TakeHit();
    }

    public void SetAnimationState(string animationName)
    {
        animator.Play(animationName);
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
        Handles.DrawAAPolyLine(transform.position, currentField.transform.position);
    }
}
