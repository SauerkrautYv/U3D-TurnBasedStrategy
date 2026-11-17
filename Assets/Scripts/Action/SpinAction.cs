using UnityEngine;
using System;
using System.Collections.Generic;

public class SpinAction : BaseAction
{
    //public delegate void SpinCompleteDelegate();
    //private SpinCompleteDelegate onSpinComplete;

    [SerializeField] private float spinSpeed = 360f;

    private float totalSpinAmount = 0f;

    private void Update()
    {
        if (!isActive) return;

        float spinAmount = spinSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up, spinAmount);

        totalSpinAmount += spinAmount;

        if (totalSpinAmount >= 360f)
        {
            totalSpinAmount = 0f;
            ActionComplete();
        }
    }


    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
    }

    public override string GetActionName()
    {
        return "旋转";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPosition };
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }
}
