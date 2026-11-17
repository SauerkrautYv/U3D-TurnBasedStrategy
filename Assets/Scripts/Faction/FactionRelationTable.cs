using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FactionRelationTable
{
    [SerializeField] private FactionId[] factions;
    [SerializeField] private FactionRelationRow[] rows;

    [Tooltip("查不到阵营或未配置时返回的默认关系")]
    [SerializeField] private FactionRelation defaultRelation = FactionRelation.中立;

    // 运行时缓存，不参与序列化
    [NonSerialized] private FactionRelation[,] lookup;
    [NonSerialized] private Dictionary<FactionId, int> indexMap;

    // ---------- 对外查询 ----------

    public FactionRelation GetRelation(FactionId a, FactionId b)
    {
        EnsureLookup();

        // 自身关系：默认走表；若表未配置则回退 友好
        if (a == b)
        {
            if (indexMap.TryGetValue(a, out int selfIdx))
                return lookup[selfIdx, selfIdx];
            return FactionRelation.友好;
        }

        if (!indexMap.TryGetValue(a, out int ia) ||
            !indexMap.TryGetValue(b, out int ib))
            return defaultRelation;

        return lookup[ia, ib];
    }

     public bool IsHostile(FactionId a, FactionId b)
        => GetRelation(a, b) == FactionRelation.敌对;

    public bool IsFriendly(FactionId a, FactionId b)
        => GetRelation(a, b) == FactionRelation.友好;

    public bool IsNeutral(FactionId a, FactionId b)
        => GetRelation(a, b) == FactionRelation.中立;

    // ---------- 供编辑器 / SO 调用 ----------

    /// <summary>强制重建缓存。SO 的 OnEnable/OnValidate 里调用。</summary>
    public void RebuildLookup()
    {
        lookup = null;
        indexMap = null;
        EnsureLookup();
    }

    /// <summary>供矩阵编辑器写入单条关系。</summary>
    public void SetRelation(FactionId a, FactionId b, FactionRelation relation, bool symmetric = true)
    {
        if (!TryGetRow(a, out var rowA) || !TryGetRow(b, out var rowB)) return;
        if (!indexMap.TryGetValue(b, out int ib) || !indexMap.TryGetValue(a, out int ia)) return;

        rowA.relationsToOthers[ib] = relation;
        if (symmetric && a != b)
            rowB.relationsToOthers[ia] = relation;

        RebuildLookup();
    }

    /// <summary>把所有关系填充为指定值，用于新建表初始化。</summary>
    public void FillAll(FactionRelation relation)
    {
        if (rows == null) return;
        int n = factions?.Length ?? 0;
        foreach (var row in rows)
        {
            row.relationsToOthers = new FactionRelation[n];
            for (int i = 0; i < n; i++) row.relationsToOthers[i] = relation;
        }
        RebuildLookup();
    }

    // ---------- 内部 ----------

    private void EnsureLookup()
    {
        if (lookup != null) return;

        int n = factions?.Length ?? 0;
        lookup = new FactionRelation[n, n];
        indexMap = new Dictionary<FactionId, int>(n);

        for (int i = 0; i < n; i++)
            indexMap[factions[i]] = i;

        for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            lookup[i, j] = defaultRelation;

        if (rows == null) return;

        foreach (var row in rows)
        {
            if (row == null || !indexMap.TryGetValue(row.faction, out int ri)) continue;
            if (row.relationsToOthers == null) continue;

            int len = Mathf.Min(row.relationsToOthers.Length, n);
            for (int j = 0; j < len; j++)
                lookup[ri, j] = row.relationsToOthers[j];
        }
    }

    private bool TryGetRow(FactionId faction, out FactionRelationRow row)
    {
        row = null;
        if (rows == null) return false;
        foreach (var r in rows)
        {
            if (r != null && r.faction == faction) { row = r; return true; }
        }
        return false;
    }

     // 编辑器与运行时只读访问
    public IReadOnlyList<FactionId> Factions => factions ?? Array.Empty<FactionId>();
    public IReadOnlyList<FactionRelationRow> Rows => rows ?? Array.Empty<FactionRelationRow>();

    // 从 FactionId 枚举生成 factions（排除 None）
    public void InitializeFactionsFromEnum()
    {
        var values = (FactionId[])Enum.GetValues(typeof(FactionId));
        var list = new List<FactionId>(values.Length);
        foreach (var v in values)
            if (v != FactionId.None) list.Add(v);

        factions = list.ToArray();
        SyncRows();
    }

    // 根据 factions 重建 rows，尽量保留已有配置
    public void SyncRows(FactionRelation defaultFill = FactionRelation.敌对)
    {
        var oldRows = rows;
        int n = factions?.Length ?? 0;
        var newRows = new FactionRelationRow[n];

        for (int i = 0; i < n; i++)
        {
            var row = new FactionRelationRow
            {
                faction = factions[i],
                relationsToOthers = new FactionRelation[n]
            };
            for (int j = 0; j < n; j++) row.relationsToOthers[j] = defaultFill;
            newRows[i] = row;
        }

        if (oldRows != null)
        {
            foreach (var old in oldRows)
            {
                if (old == null || old.relationsToOthers == null) continue;
                int i = Array.IndexOf(factions, old.faction);
                if (i < 0) continue;
                int len = Mathf.Min(n, old.relationsToOthers.Length);
                for (int j = 0; j < len; j++)
                    newRows[i].relationsToOthers[j] = old.relationsToOthers[j];
            }
        }

        rows = newRows;
        RebuildLookup();
    }
}

[Serializable]
public class FactionRelationRow
{
    public FactionId faction;
    public FactionRelation[] relationsToOthers; // 与 factions 顺序对齐
}