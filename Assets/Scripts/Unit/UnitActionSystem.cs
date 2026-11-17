using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }

    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;


    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    public void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }
    public void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    public Unit GetSelectedUnit() => selectedUnit;

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
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }

    private void Update()
    {

        if (isBusy) return;

        //当前回合不是玩家
        if (!TurnSystem.Instance.IsPlayeTurn()) return;

        //
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //选中单位
        if (TryHandleSelectedUnit()) return;

        //单位行动
        HandleSelectedAction();


    }

    //尝试选中单位
    private bool TryHandleSelectedUnit()
    {
        if (InputManager.Instance.IsMouseLeftButtonDown())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    //选中同一个单位
                    if (unit == selectedUnit) return false;

                    //单位不是当前回合行动者的单位
                    if (!unit.IsCurrnetControllerUnit(TurnSystem.Instance.GetCurrentTurnController())) return false;

                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }

        return false;
    }

    //令单位行动
    private void HandleSelectedAction()
    {
        if (selectedAction == null) return;

        if (InputManager.Instance.IsMouseLeftButtonDown())
        {
            Vector3 mouseWorldPosition = MouseWorld.Instance.GetMouseWorldPosition();
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(mouseWorldPosition);
            //位置可用
            if (!selectedAction.IsValidMovePosition(mouseGridPosition)) return;
            //COST
            if (!selectedUnit.TrySpendActionPointsToAction(selectedAction)) return;

            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, () => { ClearBusy(); });

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }



    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        selectedAction = selectedUnit.GetAction<MoveAction>();

        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

}
