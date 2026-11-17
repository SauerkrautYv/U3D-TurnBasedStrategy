using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class TurnSystemUI : MonoBehaviour
{

    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject otherTurnTipGameObject;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateTurnNumberText();
        UpdateTurnTip();
        UpdateEndTurnButtonVisibility();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnNumberText();
        UpdateTurnTip();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnTip()
    {
        otherTurnTipGameObject.SetActive(!TurnSystem.Instance.IsPlayeTurn());
    }

    private void UpdateTurnNumberText()
    {
        turnNumberText.text = "回合 " + TurnSystem.Instance.GetTurnNumber();
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayeTurn());
    }
}
