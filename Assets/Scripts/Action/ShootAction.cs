using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootUnit;
    }

    private enum State
    {
        瞄准,
        射击,
        结束动作,
    }

    private State state;
    private float stateTimer;

    private Unit targetUnit;
    private bool canShootBullet;


    [SerializeField] private LayerMask obstaclesLayerMask;
    [Header("单位动作参数")]
    [SerializeField] private int maxShootDistance = 6;
    [SerializeField] private float rotationSpeed = 10f;

    public Unit GetTargetUnit() => targetUnit;
    public int GetMaxShootDistance() => maxShootDistance;

    private void Update()
    {
        if (!isActive) return;

        switch (state)
        {
            case State.瞄准:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.fixedDeltaTime * rotationSpeed);
                break;
            case State.射击:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.结束动作:
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
            case State.瞄准:
                state = State.射击;
                stateTimer = 0.1f;
                break;
            case State.射击:
                state = State.结束动作;
                stateTimer = 0.5f;
                break;
            case State.结束动作:
                ActionComplete();
                break;
        }
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootUnit = unit
        });

        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootUnit = unit
        });
        targetUnit.Damage(40);
    }

    public override string GetActionName()
    {
        return "射击";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validMoveGridPositionList = new List<GridPosition>();

        for (int x = -Mathf.FloorToInt(maxShootDistance); x <= Mathf.FloorToInt(maxShootDistance); x++)
        {
            for (int z = -Mathf.FloorToInt(maxShootDistance); z <= Mathf.FloorToInt(maxShootDistance); z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                //修正距离为X+Z 抛弃超出的格子
                if (Mathf.Abs(x) + Mathf.Abs(z) > maxShootDistance)
                {
                    continue;
                }

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

                //有障碍阻挡
                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(
                    unitWorldPosition + Vector3.up * unitShoulderHeight,
                    shootDir,
                    Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                    obstaclesLayerMask
                ))
                {
                    continue;
                }

                validMoveGridPositionList.Add(testGridPosition);
            }
        }

        return validMoveGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action OnActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.瞄准;
        stateTimer = 0.7f;

        canShootBullet = true;

        ActionStart(OnActionComplete);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100,
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
}
