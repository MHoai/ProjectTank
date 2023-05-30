using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Complete
{
public class EliteChildState : State
{

    public GameObject m_ChildPrefab;
    [HideInInspector] public GameObject m_ChildInstance;
    private StateManager m_ChildStateManager;

    public override State RunCurrentState(int m_TypeMob, Transform PlayerLocation, GameObject m_Instance, Transform m_MobSpawner, Transform m_MobGoTo, bool oneTankLeft) 
    {
        Debug.Log("Im here");
        m_ChildInstance = Instantiate(m_ChildPrefab, m_Instance.transform.position, m_Instance.transform.rotation) as GameObject;
        m_ChildStateManager = m_ChildInstance.GetComponent<StateManager> ();
        m_ChildStateManager.currentState = m_ChildInstance.GetComponentInChildren<IdleState> ();

        m_ChildStateManager.PlayerLocation = PlayerLocation;
        m_ChildStateManager.m_Instance = m_Instance;
        m_ChildStateManager.m_MobSpawner = m_MobSpawner;
        m_ChildStateManager.m_GoTo = m_MobGoTo;
        m_ChildStateManager.oneTankLeft = oneTankLeft;
        addTank++;

        m_Instance.SetActive (false);
        return this;
    }
}
}
