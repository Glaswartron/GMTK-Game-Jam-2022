using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public enum PlayerState { Idle, Moving, WaitingForInput, Attacking, HoldingUpItem }

public enum Direction { DOWN, UP, RIGHT, LEFT }

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public float movementSpeed;
    public Field currentField;
    private Field targetField;
    public PlayerState currentState;

    public bool sword = false;
    public bool brett = false;
    public bool sandboots = false;

    public int sandBootsBonus = 1;
    public int brettBonus = 1;
    public int swordBonus = 1;

    private int currentRange;

    private Vector2 target;

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
                // Wenn keine der Animationen bereits läuft...
                if (!System.Array.Exists(new string[] { "MoveDown", "MoveUp", "MoveRight", "MoveLeft" },
                    animState => animator.GetCurrentAnimatorStateInfo(0).IsName(animState)))
                {
                    // ...wähle eine passend für die Richtung aus...
                    Vector2 playerToTarget = target - (Vector2)transform.position;
                    Direction playerToTargetDir = GetDirectionFromVector(playerToTarget);
                    switch (playerToTargetDir)
                    {
                        // ...und spiele sie
                        case Direction.DOWN:
                            animator.Play("MoveDown", 0);
                            break;
                        case Direction.UP:
                            animator.Play("MoveUp", 0);
                            break;
                        case Direction.RIGHT:
                            animator.Play("MoveRight", 0);
                            break;
                        case Direction.LEFT:
                            animator.Play("MoveLeft", 0);
                            break;
                    }
                }
                break;
            case PlayerState.Idle:
                // Wenn die Idle-Animation nicht läuft, spiele sie
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("IdleDown"))
                {
                    Debug.Log("Hello there");
                    animator.Play("IdleDown", 0);
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
                // Wenn die Idle-Animation nicht läuft, spiele sie
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("IdleDown"))
                {
                    animator.Play("IdleDown", 0);
                }
                break;
            case PlayerState.HoldingUpItem:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerObtainItem"))
                {
                    animator.Play("PlayerObtainItem", 0);
                }
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

    public void RollDice(int diceResultFromGM)
    {
        if (currentState != PlayerState.Idle)
            return;

        //WICHTIG: Die SelectDice() Methode muss vor dieser Methode aufgerufen werden! Über die UI oder so wahrscheinlich einfach

        currentRange = diceResultFromGM;

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
                    Debug.Log(field.movementCost - sandBootsBonus);
                    return (field.movementCost - sandBootsBonus) <= currentRange;
                }
                break;
            case FieldType.Water:
                if(brett)
                {
                    return (field.movementCost - brettBonus) <= currentRange;
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
            field.FieldAction();

        currentField = field;

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

    private void OnEnemyFieldReached(Field field)
    {
        EnemyField enemyField = field as EnemyField;

        StartCoroutine(AttackCo(enemyField));
    }

    private IEnumerator AttackCo(EnemyField enemyField)
    {
        currentState = PlayerState.Attacking;

        // Die Zahl an Schlägen bis die Moves verbaucht sind oder der Gegner besiegt ist
        int hits = Mathf.Min(currentRange, enemyField.stats.GetHP()); 

        // Für jeden Hit einmal die Animation abspielen und für die Länge der Animation warten
        for (int i = 0; i < hits; i++)
        {
            // Animation auswählen je nach Richtung
            Vector2 playerToEnemy = (Vector2)enemyField.transform.position - (Vector2)transform.position;
            Direction playerToEnemyDir = GetDirectionFromVector(playerToEnemy);
            switch (playerToEnemyDir)
            {
                case Direction.DOWN:
                    animator.Play("PlayerAttackDown", 0);
                    break;
                case Direction.UP:
                    animator.Play("PlayerAttackUp", 0);
                    break;
                case Direction.RIGHT:
                    animator.Play("PlayerAttackRight", 0);
                    break;
                case Direction.LEFT:
                    animator.Play("PlayerAttackLeft", 0);
                    break;
            }

            float animDuration = animator.GetCurrentAnimatorStateInfo(0).length;

            currentRange--;

            yield return new WaitForSeconds(animDuration);
        }

        // Gegner besiegt => Gehe auf das Feld des Gegners
        if (enemyField.stats.IsDead())
        {
            targetField = enemyField;
            MoveTo(enemyField.transform.position, OnFieldReached);
        }
        else // Gegner nicht besiegt => Gehe zurück auf das Feld wo du herkommst
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

    public void HoldUpItem(bool holdUp)
    {
        if (holdUp)
            currentState = PlayerState.HoldingUpItem;
        else
            currentState = PlayerState.Idle;
    }

    private void SetAnimationState(string animationName)
    {
        animator.Play(animationName, 0);
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

    /// <summary>
    /// Bestimmt eindeutig die Richtung, in die ein Vektor zeigt (als Direction). 
    /// Wenn Nullvektor: DOWN
    /// </summary>
    /// <param name="vec">Der Vektor, dessen Richtung bestimmt werden soll</param>
    /// <returns>Die Richtung, in die der Vektor zeigt (UP, DOWN, LEFT, RIGHT)</returns>
    public static Direction GetDirectionFromVector(Vector2 vec)
    {
        float absMoveX = Mathf.Abs(vec.x);
        float absMoveY = Mathf.Abs(vec.y);

        /* Ähnlich dem, was der Blend Tree macht, nur dass 
         * eine eindeutige Richtung rauskommt */
        if (vec.x > 0 && absMoveX > absMoveY)
            return Direction.RIGHT;
        else if (vec.y > 0 && absMoveY > absMoveX)
            return Direction.UP;
        else if (vec.x < 0 && absMoveX > absMoveY)
            return Direction.LEFT;
        else if (vec.y < 0 && absMoveY > absMoveX)
            return Direction.DOWN;
        else
            return Direction.DOWN;
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
