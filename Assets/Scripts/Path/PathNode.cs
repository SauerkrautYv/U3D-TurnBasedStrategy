using UnityEngine;

public class PathNode
{
    private GridPosition gridPosition;
    private int GCost;
    private int HCost;
    private int FCost;

    private PathNode cameFromPathNode;

    private bool isWalkable = true;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return gridPosition.ToString();
    }

    public GridPosition GetGridPosition() => gridPosition;

    public int GetHCost() => HCost;
    public int GetGCost() => GCost;
    public int GetFCost() => FCost;

    public void SetHCost(int cost) => HCost = cost;
    public void SetGCost(int cost) => GCost = cost;
    public void SetFCost(int cost) => FCost = cost;

    public void CalculateFCost() => FCost = GCost + HCost;

    public void SetCameFromPathNode(PathNode pathNode) => cameFromPathNode = pathNode;
    public void ResetCameFromPathNode() => cameFromPathNode = null;
    public PathNode GetCameFromPathNode() => cameFromPathNode;

    public bool IsWalkable() => isWalkable;

    public void SetIsWalkable(bool isWalkable) => this.isWalkable = isWalkable;
}
