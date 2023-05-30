using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Complete
{
public class BombChaseState : State
{
    public BombAttackState attackState;
    public BombIdleState idleState;
    
    public NavMeshAgent agent;

    public override State RunCurrentState(int m_TypeMob, Transform PlayerLocation, GameObject m_Instance, Transform m_MobSpawner, Transform m_MobGoTo, bool oneTankLeft) 
    {
        MobMovement m_Movement = m_Instance.GetComponent<MobMovement> ();
        Vector3 vector_mob_to_spawn = m_MobSpawner.position - transform.position;
        float distance_mob_spawn = vector_mob_to_spawn.magnitude;

        Vector3 vector_mob_to_goto = m_MobGoTo.position - transform.position;
        float distance_mob_goto = vector_mob_to_goto.magnitude;

        Vector3 center_point = (m_MobSpawner.position + m_MobGoTo.position)/2;
        Vector3 temp = transform.position - center_point;
        float radius_range = temp.magnitude;

        if (radius_range > 20f)
        {
            idleState.m_DetectPlayer = 0f;
            return idleState;
        }
        else
        {
            Vector3 vector_mob_to_player = PlayerLocation.position - transform.position;
            float distance_player_mob = vector_mob_to_player.magnitude;
            float distance_mob_desired_place = distance_player_mob - 2f;
            float k = distance_mob_desired_place / distance_player_mob;
            Vector3 vector_mob_to_desired_place = vector_mob_to_player * k;
            Vector3 desired_place = vector_mob_to_desired_place + transform.position;

            m_Movement.Move_enemy(desired_place);

            if (distance_player_mob < 2f + 0.1f &&  distance_player_mob > 2f - 0.1f)
            {
                return attackState;
            }

            return this;
        }
    }
    
    private void MoveTo(Vector3 desired_place)
    {
        agent.SetDestination(desired_place);
    }
}
}
