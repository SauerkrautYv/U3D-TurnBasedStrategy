using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        等待敌人回合,
        进行回合,
        忙碌
    }

    [SerializeField] private FactionId AIFaction;

    private State state;

    private float timer;

    private void Awake()
    {
        state = State.等待敌人回合;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        //if (TurnSystem.Instance.IsPlayeTurn()) return;
        if (TurnSystem.Instance.GetCurrentTurnController() != AIFaction) return;

        switch (state)
        {
            case State.等待敌人回合:
                break;
            case State.进行回合:
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.忙碌;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.忙碌:
                break;
        }


    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.进行回合;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.GetCurrentTurnController() == AIFaction)
        {
            state = State.进行回合;
            timer = 2f;
        }
    }

    private bool TryTakeEnemyAIAction(Action OnEnemyAIActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetFactionUnitList(AIFaction))
        {
            if (TryTakeEnemyAIAction(enemyUnit, OnEnemyAIActionComplete))
            {

                return true;
            }
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action OnEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendActionPointsToAction(baseAction))
            {
                //费用不足
                continue;
            }

            if (bestEnemyAIAction == null)
            {

                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if(bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, () => { OnEnemyAIActionComplete(); });
            return true;
        }
        else
        {
            return false;
        }
    }
}
