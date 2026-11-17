using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointText;
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;

    private void Start()
    {
        unit.OnActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnHealthChange += HealthSystem_OnHealthChange;

        UpdateActionPointsText();
        UpdateHealthBar();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void HealthSystem_OnHealthChange(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void UpdateActionPointsText()
    {
        actionPointText.text = unit.GetActionPoints().ToString();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthPointNormalized();
    }

}
