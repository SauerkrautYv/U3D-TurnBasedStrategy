using System;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public static event EventHandler OnAnySwordHit;

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    private enum State
    {
        挥剑,
        命中,
    }

    private State state;
    private float stateTimer;

    private Unit targetUnit;

    [Header("单位动作参数")]
    [SerializeField] private int maxSwordDistance = 1;
    [SerializeField] private float rotationSpeed = 10f;

    public int GetMaxSwordDistance() => maxSwordDistance;

    private void Update()
    {
        if (!isActive) return;

        switch (state)
        {
            case State.挥剑:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.fixedDeltaTime * rotationSpeed);
                break;
            case State.命中:
                break;
        }

        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
            NextState();
    }

    private void NextState()
    {
        switch (state)
        {
            case State.挥剑:
                state = State.命中;
                stateTimer = 0.5f;
                targetUnit.Damage(100);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.命中:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "近战";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validMoveGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -Mathf.FloorToInt(maxSwordDistance); x <= Mathf.FloorToInt(maxSwordDistance); x++)
        {
            for (int z = -Mathf.FloorToInt(maxSwordDistance); z <= Mathf.FloorToInt(maxSwordDistance); z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                // 检查鼠标点击的网格位置是否有效
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                // 检查目标网格位置是否有单位
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                //多玩家后敌人阵营后需要修改
                //是否一个阵营
                if (targetUnit.GetFactionId() == unit.GetFactionId())
                {
                    continue;
                }


                validMoveGridPositionList.Add(testGridPosition);
            }
        }

        return validMoveGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.挥剑;
        stateTimer = 0.7f;


        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }
}
