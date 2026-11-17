using UnityEngine;
[CreateAssetMenu(menuName = "阵营/关系表", fileName = "阵营关系表SO")]
public class FactionRelationTableSO : ScriptableObject
{
    [SerializeField] private FactionRelationTable factionRelationTable;

    public FactionRelationTable Table => factionRelationTable;

    private void OnEnable()
    {
        if (factionRelationTable == null)
            factionRelationTable = new FactionRelationTable();
        factionRelationTable?.RebuildLookup();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        factionRelationTable?.RebuildLookup();
    }

    public void ResetTable()
    {
        factionRelationTable = new FactionRelationTable();
        factionRelationTable.InitializeFactionsFromEnum();
    }
#endif
}
