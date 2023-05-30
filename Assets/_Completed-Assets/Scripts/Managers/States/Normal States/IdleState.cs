using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Complete
{
public class IdleState : State
{
    public ChaseState chaseState;
    public bool CanSeeThePlayer;
    public NavMeshAgent agent;
    bool lockSpawn = false;
    bool lockGoTo = true;
    public bool lockRecovery = true;

    [HideInInspector] public float m_DetectPlayer = 20f;

    public override State RunCurrentState(int m_TypeMob, Transform PlayerLocation, GameObject m_Instance, Transform m_MobSpawner, Transform m_MobGoTo, bool oneTankLeft) 
    {
        TankHealth targetHealth = m_Instance.GetComponent<TankHealth> ();

        Vector3 vector_mob_to_player = PlayerLocation.position - transform.position;
        float distance_player_mob = vector_mob_to_player.magnitude;

        if ((transform.position.x > m_MobSpawner.position.x -0.1f && transform.position.x < m_MobSpawner.position.x + 0.1f &&
            transform.position.z > m_MobSpawner.position.z -0.1f && transform.position.z < m_MobSpawner.position.z + 0.1f)

            || (transform.position.x > m_MobGoTo.position.x -0.1f && transform.position.x < m_MobGoTo.position.x + 0.1f &&
            transform.position.z > m_MobGoTo.position.z -0.1f && transform.position.z < m_MobGoTo.position.z + 0.1f)
            )
        {
            m_DetectPlayer = 20f;
        }

        if (distance_player_mob < m_DetectPlayer || targetHealth.GetCurrentHealth() < 100f)
        {
            return chaseState;
        }
        else
        {
            if (!(transform.position.x > m_MobSpawner.position.x -0.1f && transform.position.x < m_MobSpawner.position.x + 0.1f &&
                transform.position.z > m_MobSpawner.position.z -0.1f && transform.position.z < m_MobSpawner.position.z + 0.1f) && lockSpawn)
            {
                agent.SetDestination(m_MobSpawner.position);
                lockGoTo = false;
            }
            else
            {
                lockGoTo = true;
            }
            
            if (lockGoTo)
            {
                agent.SetDestination(m_MobGoTo.position);
                lockSpawn = false;
            }

            if ((transform.position.x > m_MobGoTo.position.x -0.1f && transform.position.x < m_MobGoTo.position.x + 0.1f &&
                transform.position.z > m_MobGoTo.position.z -0.1f && transform.position.z < m_MobGoTo.position.z + 0.1f) && lockGoTo)
            {
                lockSpawn = true;
                lockGoTo = false;
            }

            return this;
        }
    }
}
}
