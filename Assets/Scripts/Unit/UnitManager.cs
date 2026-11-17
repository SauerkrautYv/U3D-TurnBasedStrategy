using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<Unit> unitList;

    private Dictionary<FactionId, List<Unit>> factionUnitDictionary;

    public IReadOnlyList<Unit> GetUnitList() => unitList;
    public IReadOnlyList<Unit> GetFactionUnitList(FactionId factionId)
    {
        return factionUnitDictionary.TryGetValue(factionId, out var list) ? list : null;
    }

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

        unitList = new List<Unit>();
        factionUnitDictionary = new Dictionary<FactionId, List<Unit>>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void OnDestroy()
    {
        Unit.OnAnyUnitSpawned -= Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
        if (Instance == this) Instance = null;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        AddUnit(unit);

    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        RemoveUnit(unit);

        //

    }

    private void AddUnit(Unit unit)
    {
        if (!unitList.Contains(unit)) unitList.Add(unit);
        var list = GetOrCreateFactionList(unit.GetFactionId());
        if (!list.Contains(unit)) list.Add(unit);
    }

    private List<Unit> GetOrCreateFactionList(FactionId factionId)
    {
        if (!factionUnitDictionary.TryGetValue(factionId, out var list))
        {
            list = new List<Unit>();
            factionUnitDictionary[factionId] = list;
        }
        return list;
    }

    private void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
        if (factionUnitDictionary.TryGetValue(unit.GetFactionId(), out var list))
            list.Remove(unit);
    }

}
