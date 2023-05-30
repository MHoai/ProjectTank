using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
public class StateManager : MonoBehaviour
{
    public State currentState;
    public int m_TypeMob;
    [HideInInspector] public int addTank;

    [HideInInspector] public Transform PlayerLocation;
    [HideInInspector] public GameObject m_Instance;
    [HideInInspector] public Transform m_MobSpawner;
    [HideInInspector] public bool oneTankLeft;
    [HideInInspector] public Transform m_GoTo;


    // Update is called once per frame
    void Update()
    {
        UpdateTankNumber();
        RunCurrentState();
    }

    void UpdateTankNumber()
    {
        addTank = currentState.addTank;
    }

    private void RunCurrentState()
    {
        State nextState = currentState?.RunCurrentState(m_TypeMob, PlayerLocation, m_Instance, m_MobSpawner, m_GoTo, oneTankLeft);

        if (nextState != null)
        {
            SwitchToNextState(nextState);
        }
    }

    private void SwitchToNextState(State nextState)
    {
        currentState = nextState;
    }
}
}
