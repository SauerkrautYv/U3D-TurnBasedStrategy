using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FactionRelationTableSO))]
public class FactionRelationTableSOEditor : Editor
{
    private const float LabelWidth = 110f;
    private const float CellWidth = 100f;

    private bool symmetric = true;

    public override void OnInspectorGUI()
    {
        var so = (FactionRelationTableSO)target;
        var table = so.Table;

        if (table == null)
        {
            EditorGUILayout.HelpBox("关系表为空。点下面的按钮重建，或检查 SO 初始化。", MessageType.Error);
            if (GUILayout.Button("重建关系表"))
            {
                Undo.RecordObject(so, "重建关系表");
                so.ResetTable();
                EditorUtility.SetDirty(so);
            }
            return;
        }

        DrawToolbar(so, table);
        EditorGUILayout.Space(8);
        DrawMatrix(so, table);
        EditorGUILayout.Space(8);
        DrawValidate(so, table);
    }

    // ---------- 工具栏 ----------

    private void DrawToolbar(FactionRelationTableSO so, FactionRelationTable table)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("从枚举初始化阵营", EditorStyles.toolbarButton))
        {
            Undo.RecordObject(so, "初始化阵营");
            table.InitializeFactionsFromEnum();
            table.RebuildLookup();
            EditorUtility.SetDirty(so);
        }

        if (GUILayout.Button("同步行结构", EditorStyles.toolbarButton))
        {
            Undo.RecordObject(so, "同步行");
            table.SyncRows();
            EditorUtility.SetDirty(so);
        }

        if (GUILayout.Button("全部填充 中立", EditorStyles.toolbarButton))
        {
            Undo.RecordObject(so, "填充 中立");
            table.FillAll(FactionRelation.中立);
            EditorUtility.SetDirty(so);
        }

        GUILayout.FlexibleSpace();
        symmetric = GUILayout.Toggle(symmetric, "对称模式", EditorStyles.toolbarButton, GUILayout.Width(70));

        EditorGUILayout.EndHorizontal();
    }

    // ---------- 矩阵 ----------

    private void DrawMatrix(FactionRelationTableSO so, FactionRelationTable table)
    {
        var factions = table.Factions;
        int n = factions.Count;

        if (n == 0)
        {
            EditorGUILayout.HelpBox("还没有阵营。点“从枚举初始化阵营”，或手动在 factions 里配置。", MessageType.Info);
            return;
        }

        // 表头
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(LabelWidth));
        for (int j = 0; j < n; j++)
            GUILayout.Label(factions[j].ToString(), EditorStyles.boldLabel, GUILayout.Width(CellWidth));
        EditorGUILayout.EndHorizontal();

        // 矩阵主体
        for (int i = 0; i < n; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(factions[i].ToString(), EditorStyles.boldLabel, GUILayout.Width(LabelWidth));

            for (int j = 0; j < n; j++)
            {
                var cur = table.GetRelation(factions[i], factions[j]);
                bool editable = !symmetric || i <= j;

                var next = DrawCell(cur, editable);

                if (editable && next != cur)
                {
                    Undo.RecordObject(so, "修改阵营关系");
                    table.SetRelation(factions[i], factions[j], next, symmetric);
                    table.RebuildLookup();
                    EditorUtility.SetDirty(so);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private FactionRelation DrawCell(FactionRelation cur, bool editable)
    {
        var oldColor = GUI.backgroundColor;
        GUI.backgroundColor = GetColor(cur);

        FactionRelation result;
        if (editable)
        {
            result = (FactionRelation)EditorGUILayout.EnumPopup(cur, GUILayout.Width(CellWidth));
        }
        else
        {
            GUI.enabled = false;
            EditorGUILayout.EnumPopup(cur, GUILayout.Width(CellWidth));
            GUI.enabled = true;
            result = cur;
        }

        GUI.backgroundColor = oldColor;
        return result;
    }

    private static Color GetColor(FactionRelation r)
    {
        switch (r)
        {
            case FactionRelation.敌对: return new Color(1f, 0.55f, 0.55f);
            case FactionRelation.友好: return new Color(0.55f, 1f, 0.55f);
            default: return Color.white;
        }
    }

    // ---------- 校验 ----------

    private void DrawValidate(FactionRelationTableSO so, FactionRelationTable table)
    {
        if (!GUILayout.Button("校验关系表")) return;

        bool ok = true;
        var factions = table.Factions;
        var rows = table.Rows;
        int n = factions.Count;

        if (rows.Count != n)
        {
            Debug.LogWarning($"[阵营表] rows 数量 {rows.Count} 与 factions 数量 {n} 不一致，请点“同步行结构”。");
            ok = false;
        }

        for (int i = 0; i < n; i++)
        {
            var row = rows[i];
            if (row == null) { Debug.LogWarning($"[阵营表] 第 {i} 行为 null。"); ok = false; continue; }
            if (row.faction != factions[i])
                Debug.LogWarning($"[阵营表] 第 {i} 行 faction={row.faction}，与 factions[{i}]={factions[i]} 不一致。");
            if (row.relationsToOthers == null || row.relationsToOthers.Length != n)
            {
                Debug.LogWarning($"[阵营表] 行 {row.faction} 的 relationsToOthers 长度错误。");
                ok = false;
            }
        }

        for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)
            {
                var a = table.GetRelation(factions[i], factions[j]);
                var b = table.GetRelation(factions[j], factions[i]);
                if (a != b)
                    Debug.LogWarning($"[阵营表] 非对称：{factions[i]}→{factions[j]}={a}，{factions[j]}→{factions[i]}={b}。");
            }

        if (ok) Debug.Log("[阵营表] 校验通过。");
    }
}