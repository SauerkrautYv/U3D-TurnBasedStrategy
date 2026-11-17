using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    [Header("单位动作参数")]
    [SerializeField] private int maxInteractDistance = 1;


    private void Update()
    {
        if (!isActive) return;

    }

    public override string GetActionName()
    {
        return "交互";
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

        for (int x = -Mathf.FloorToInt(maxInteractDistance); x <= Mathf.FloorToInt(maxInteractDistance); x++)
        {
            for (int z = -Mathf.FloorToInt(maxInteractDistance); z <= Mathf.FloorToInt(maxInteractDistance); z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                // 检查鼠标点击的网格位置是否有效
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
                if (interactable == null)
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
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);

        interactable.Interact(OnInteractComplete);

        ActionStart(onActionComplete);
    }

    private void OnInteractComplete()
    {
        ActionComplete();
    }
}
