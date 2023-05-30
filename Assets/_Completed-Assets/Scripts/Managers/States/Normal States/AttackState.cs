using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{
public class AttackState : State
{
    public ChaseState chaseState;

    public override State RunCurrentState(int m_TypeMob, Transform PlayerLocation, GameObject m_Instance, Transform m_MobSpawner, Transform m_MobGoTo, bool oneTankLeft) 
    {
        MobShooting m_Shooting = m_Instance.GetComponent<MobShooting> ();
        Vector3 vector_mob_to_player = PlayerLocation.position - transform.position;
        float distance_player_mob = vector_mob_to_player.magnitude;
        float turn = Vector3.Angle(vector_mob_to_player, transform.forward);

        if (!(distance_player_mob < 10f + 0.1f &&  distance_player_mob > 10f - 0.1f) || turn >= 3f)
        {
            m_Shooting.setToMin();
            return chaseState;
        }
        else if (!oneTankLeft)
        {
            m_Shooting.Shoot(m_TypeMob);
            return this;
        }
        return this;
    }
}
}
