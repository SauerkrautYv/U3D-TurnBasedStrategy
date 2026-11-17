using System;
using UnityEngine;


public class Unit : MonoBehaviour
{
    private const int START_ACTION_POINT = 2;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    public event EventHandler OnActionPointsChanged;


    [SerializeField] private FactionMember factionMember;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private int actionPoints = 2;

    public int GetActionPoints() => actionPoints;
    public void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    public void ResetActionPoints()
    {
        actionPoints = START_ACTION_POINT;
        OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool CanSpendActionPointsToAction(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TrySpendActionPointsToAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();

    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitOnGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);

    }

    private void Update()
    {
        // 更新单位在网格上的位置
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }

    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if(baseAction is T)
            {
                return (T)baseAction;
            }
        }

        return null;
    }


    public FactionId GetFactionId() => factionMember.GetFactionId;

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool IsCurrnetControllerUnit(FactionId checkControllerType)
    {
        return factionMember.GetFactionId == checkControllerType;
    }

    //回合变化 重置行动点
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        //是当前行动者的回合和单位
        if (IsCurrnetControllerUnit(TurnSystem.Instance.GetCurrentTurnController()))
        {
            ResetActionPoints();
        }


    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public void Damage(int damageAmount)
    {

        healthSystem.Damage(damageAmount);
    }
}
