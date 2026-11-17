using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ActionButtonUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedVisualGameObject;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        textMeshPro.text = baseAction.GetActionName().ToUpper();

        button.onClick.AddListener(() =>
            {
                UnitActionSystem.Instance.SetSelectedAction(baseAction);
            }
        );
    }

    public void UpdateSelectedVisual()
    {
        selectedVisualGameObject.SetActive(UnitActionSystem.Instance.GetSelectedAction() == baseAction);
    }


}
