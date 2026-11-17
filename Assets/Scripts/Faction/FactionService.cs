using UnityEngine;

public static class FactionService
{
    private static FactionRelationTable table;

    public static void Initialize(FactionRelationTable relationTable)
    {
        table = relationTable;
    }

    public static FactionRelation GetRelation(FactionId a, FactionId b)
        => table.GetRelation(a, b);

    public static bool IsHostile(FactionId a, FactionId b)
        => GetRelation(a, b) == FactionRelation.敌对;
}