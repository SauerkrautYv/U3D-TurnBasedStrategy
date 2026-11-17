using UnityEngine;
using System.Collections.Generic;
using System;

public class MoveAction : BaseAction
{
    public event EventHandler OnMoveStart;
    public event EventHandler OnMoveStop;

    [Header("单位参数")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float maxMoveDistance = 5f;
    [Header("单位组件")]
    [SerializeField] private Animator animator;

    private List<Vector3> positionList;
    private int currentPositionIndex;


    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        //if(!isActive) return;
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 targetDirection = (targetPosition - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * moveSpeed);

        //使用四元数插值旋转
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }

        if (transform.position == targetPosition)
        {
            currentPositionIndex++;

            if (currentPositionIndex >= positionList.Count)
            {
                ActionComplete();
                OnMoveStop?.Invoke(this, EventArgs.Empty);
            }

        }


    }

    public override void TakeAction(GridPosition targetGridPosition, Action OnActionComplete)
    {
        List<GridPosition> gridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), targetGridPosition, out int pathLength);
        currentPositionIndex = 0;

        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in gridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }


        OnMoveStart?.Invoke(this, EventArgs.Empty);

        //设置激活flag
        ActionStart(OnActionComplete);
    }



    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validMoveGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -Mathf.FloorToInt(maxMoveDistance); x <= Mathf.FloorToInt(maxMoveDistance); x++)
        {
            for (int z = -Mathf.FloorToInt(maxMoveDistance); z <= Mathf.FloorToInt(maxMoveDistance); z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = LevelGrid.Instance.GetGridPosition(transform.position) + offsetGridPosition;
                // 检查鼠标点击的网格位置是否有效
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                // 检查目标网格位置是否有效
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }
                // 目标位置是否可通行
                if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }
                // 是否可以到达
                if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                // 路径是否超出长度
                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                {
                    continue;
                }

                validMoveGridPositionList.Add(testGridPosition);
            }
        }

        return validMoveGridPositionList;
    }

    public override string GetActionName()
    {
        return "移动";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }
}
