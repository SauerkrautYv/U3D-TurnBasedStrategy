using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform grenadeProjectilePrefab;
    [Header("单位动作参数")]
    [SerializeField] private int maxThrowDistance = 6;

    private void Update()
    {
        if (!isActive) return;


    }

    public override string GetActionName()
    {
        return "手榴弹";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validMoveGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -Mathf.FloorToInt(maxThrowDistance); x <= Mathf.FloorToInt(maxThrowDistance); x++)
        {
            for (int z = -Mathf.FloorToInt(maxThrowDistance); z <= Mathf.FloorToInt(maxThrowDistance); z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                //修正距离为X+Z 抛弃超出的格子
                if (Mathf.Abs(x) + Mathf.Abs(z) > maxThrowDistance)
                {
                    continue;
                }

                // 检查鼠标点击的网格位置是否有效
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
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
        Transform grenadeProjectileTransfrom = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeAction = grenadeProjectileTransfrom.GetComponent<GrenadeProjectile>();
        grenadeAction.Setup(gridPosition, OnGrenadeBehaviourComplete);

        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }
}
