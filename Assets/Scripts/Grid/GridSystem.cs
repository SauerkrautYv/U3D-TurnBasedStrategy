using System;
using UnityEngine;


public class GridSystem<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectsArray;

    //Func<GridSystem<TGridObject>, GridPosition, TGridObject> C# 内置的泛型委托，Func<A, B, C> 表示接收 A、B 两个参数，返回 C 的函数
    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObjectFunc)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectsArray = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectsArray[x, z] = createGridObjectFunc(this, gridPosition);
            }
        }
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0
        && gridPosition.z >= 0
        && gridPosition.x < width
        && gridPosition.z < height;
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
        );
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectsArray[gridPosition.x, gridPosition.z];
    }

    public int GetWidth() => width;

    public int GetHeight() => height;

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
}
