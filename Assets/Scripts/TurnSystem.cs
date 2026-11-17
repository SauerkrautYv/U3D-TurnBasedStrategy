using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set; }

    public event EventHandler OnTurnChanged;

    private int turnNumber = 1;

    [SerializeField] private FactionId playerTurnController;
    [SerializeField] private FactionId currentTurnController;
    [SerializeField] private List<FactionId> turnControllerList;
    private int currentTurnControllerIndex = 0;

    public int GetTurnNumber() => turnNumber;

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

        // 初始化
        //turnControllerList = new();
        if (turnControllerList.Count > 0)
            currentTurnControllerIndex = 0;
    }

    public void NextTurn()
    {
        if (turnControllerList.Count == 0) return;

        currentTurnControllerIndex++;

        if (currentTurnControllerIndex >= turnControllerList.Count)
        {
            currentTurnControllerIndex = 0;
            turnNumber++; // 一轮结束才 +1
        }

        currentTurnController = turnControllerList[currentTurnControllerIndex];

        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayeTurn()
    {
        return currentTurnController == playerTurnController;
    }

    public FactionId GetCurrentTurnController() => currentTurnController;
    public FactionId GetplayerTurnController() => playerTurnController;
}
