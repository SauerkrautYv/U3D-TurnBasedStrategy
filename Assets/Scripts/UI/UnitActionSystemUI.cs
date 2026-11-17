using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += OnActionStarted;

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;

        CreateUnitActionButtons();
        UpdateActionPointsText();
    }

    private void CreateUnitActionButtons()
    {
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
        {
            Transform buttonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = buttonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void OnSelectUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPointsText();
    }

    private void OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPointsText()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointsText.text = selectedUnit.GetActionPoints().ToString();
    }
}
