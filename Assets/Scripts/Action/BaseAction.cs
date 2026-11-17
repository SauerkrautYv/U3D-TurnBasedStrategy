using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStart;
    public static event EventHandler OnAnyActionCompleted;


    protected Action OnActionComplete;

    protected bool isActive = false;
    protected Unit unit;


    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public Unit GetUnit() => unit;

    public abstract string GetActionName();

    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual bool IsValidMovePosition(GridPosition gridPosition)
    {
        List<GridPosition> validMoveGridPositionList = GetValidActionGridPositionList();
        return validMoveGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    protected void ActionStart(Action OnActionComplete)
    {
        isActive = true;
        this.OnActionComplete = OnActionComplete;

        OnAnyActionStart?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        OnActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validMoveGridPositionList = GetValidActionGridPositionList();

        foreach (GridPosition gridPosition in validMoveGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);

            enemyAIActionList.Add(enemyAIAction);
        }

        enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);

        if (enemyAIActionList.Count > 0)
        {
            return enemyAIActionList[0];
        }
        else
        {
            return null;
        }
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

}
