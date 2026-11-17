using TMPro;
using UnityEngine;

public class PathNodeGridDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro GCostText;
    [SerializeField] private TextMeshPro HCostText;
    [SerializeField] private TextMeshPro FCostText;
    [SerializeField] private SpriteRenderer 可通行标识Sprite;

    private PathNode pathNode;

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        pathNode = (PathNode)gridObject;
    }

    protected override void Update()
    {
        base.Update();
        //GCostText.text = pathNode.GetGCost().ToString();
        //HCostText.text = pathNode.GetHCost().ToString();
        //FCostText.text = pathNode.GetFCost().ToString();
        GCostText.SetText("{0}", pathNode.GetGCost());
        HCostText.SetText("{0}", pathNode.GetHCost());
        FCostText.SetText("{0}", pathNode.GetFCost());
        可通行标识Sprite.color = pathNode.IsWalkable() ? Color.green : Color.red;

    }
}
