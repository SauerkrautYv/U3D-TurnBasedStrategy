using System;
using UnityEngine;

public class PathfindingUpdate : MonoBehaviour
{
    private void Start()
    {
        DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyDestroyed;
    }

    private void DestructibleCrate_OnAnyDestroyed(object sender, EventArgs e)
    {
        DestructibleCrate destructibleCreate = sender as DestructibleCrate;
        Pathfinding.Instance.SetWalkableGridPosition(destructibleCreate.GetGridPosition(), true);
    }
}
