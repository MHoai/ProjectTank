using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
public class EliteAttackState : State
{
    public EliteChaseState chaseState;
    public EliteSpecialAttackState specialAttackState;

    public override State RunCurrentState(int m_TypeMob, Transform PlayerLocation, GameObject m_Instance, Transform m_MobSpawner, Transform m_MobGoTo, bool oneTankLeft) 
    {

        MobShooting m_Shooting = m_Instance.GetComponent<MobShooting> ();
        MobMovement m_Movement = m_Instance.GetComponent<MobMovement> ();
        Vector3 vector_mob_to_player = PlayerLocation.position - transform.position;
        float distance_player_mob = vector_mob_to_player.magnitude;
        float turn = Vector3.Angle(vector_mob_to_player, transform.forward);

        if (distance_player_mob <= 15f + 0.1f && distance_player_mob >= 15f - 0.1f && !oneTankLeft)
        {
            m_Shooting.Shoot(m_TypeMob);
            return this;
        }
        else
        {
            m_Shooting.setToMin();
            return chaseState;
        }

        //return this;
    }
}
}
