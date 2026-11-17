using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    public enum GridVisualType
    {
        白,
        蓝,
        红,
        浅红,
        黄,
        绿
    }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

    [SerializeField] private Transform gridSystemVisualSinglePrefab;

    [SerializeField] private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
        ];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab,
                        LevelGrid.Instance.GetWorldPosition(gridPosition),
                        Quaternion.identity
                    );

                gridSystemVisualSingleTransform.parent = transform;

                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        LevelGrid.Instance.OnAnyUnitMoveGridPosition += LevelGrid_OnAnyUnitMoveGridPosition;

        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMoveGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    public Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("未定该颜色的网格在gridVisualTypeMaterialList" + gridVisualType);
        return null;
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        if (selectedUnit == null || selectedAction == null) return;

        GridVisualType gridVisualType;
        if (selectedAction != null)
        {
            switch (selectedAction)
            {
                case MoveAction moveAction:
                    gridVisualType = GridVisualType.绿;
                    break;
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.红;
                    ShowGridPositionRange(selectedUnit.GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.浅红);
                    break;
                case GrenadeAction grenadeAction:
                    gridVisualType = GridVisualType.黄;
                    break;
                case SwordAction swordAction:
                    gridVisualType = GridVisualType.红;
                    ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.浅红);
                    break;
                case InteractAction interactAction:
                    gridVisualType = GridVisualType.蓝;
                    break;
                case SpinAction spinAction:
                    gridVisualType = GridVisualType.蓝;
                    break;
                default:
                    gridVisualType = GridVisualType.白;
                    break;
            }

            List<GridPosition> moveGridPositionList = selectedAction.GetValidActionGridPositionList();
            ShowGridPositionList(moveGridPositionList, gridVisualType);
        }

    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    public void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                //修正距离为X+Z 抛弃超出的格子
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z]
                .Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }
}
